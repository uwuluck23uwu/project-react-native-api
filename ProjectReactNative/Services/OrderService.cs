using System.Net;
using System.Linq.Expressions;
using AutoMapper;
using StripeEvent = Stripe.Event;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ProjectReactNative.Services
{
    public class OrderService : Service<Order>, IOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<SignalHub> _hub;
        private readonly IMapper _mapper;
        private readonly StripeSettings _stripeSettings;

        // ใช้ string แทนคอนสแตนต์ของ Stripe บางเวอร์ชัน
        private const string PI_SUCCEEDED = "payment_intent.succeeded";
        private const string PI_FAILED = "payment_intent.payment_failed";

        public OrderService(
            ApplicationDbContext db,
            IHubContext<SignalHub> hub,
            IMapper mapper,
            IOptions<StripeSettings> stripeOptions
        ) : base(db, hub)
        {
            _db = db;
            _hub = hub;
            _mapper = mapper;
            _stripeSettings = stripeOptions.Value;
        }

        public async Task<ResponsePagination> GetAllOrders(int pageSize, int currentPage, string search)
        {
            // include OrderItems
            var baseResult = await GetAllAsync(
                pageSize, currentPage, search,
                new Expression<Func<Order, object>>[] { x => x.OrderItems }
            );
            return baseResult;
        }

        public async Task<ResponseData> CreateOrderAndPaymentAsync(OrderCreateDTO dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return new ResponseData(HttpStatusCode.BadRequest, false, "ไม่มีรายการสั่งซื้อ", null);

            // 1) สร้าง Order
            var order = new Order
            {
                OrderId = await GenerateRunningIdAsync("OrderId", "OR"),
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderDatetime = DateTime.UtcNow,
                PaymentStatus = "pending"
            };

            decimal total = 0m;
            int totalQty = 0;

            // 2) เพิ่มรายการ OrderItems + คำนวณยอด
            foreach (var it in dto.Items)
            {
                decimal price = it.PriceEach ?? await GetUnitPriceAsync(it.RefId);

                var oi = new OrderItem
                {
                    OrderItemId = await GenerateRunningIdAsync("OrderItemId", "OI"),
                    OrderId = order.OrderId,
                    RefId = it.RefId,
                    Quantity = it.Quantity,
                    PriceEach = price
                };

                total += price * it.Quantity;
                totalQty += it.Quantity;
                order.OrderItems.Add(oi);
            }

            order.TotalPrice = total;
            order.TotalAmount = totalQty;

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            // 3) สร้าง Stripe PaymentIntent
            var amountMinor = (long)Math.Round(total * 100); // THB -> สตางค์
            var piService = new Stripe.PaymentIntentService();
            var createOptions = new Stripe.PaymentIntentCreateOptions
            {
                Amount = amountMinor,
                Currency = dto.Currency,
                Description = dto.Description,
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", order.OrderId },
                    { "user_id", order.UserId }
                },
                AutomaticPaymentMethods = new Stripe.PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };
            var paymentIntent = await piService.CreateAsync(createOptions);

            // 4) บันทึก Payment ในระบบ
            var payment = new Payment
            {
                PaymentId = await GenerateRunningIdAsync("PaymentId", "PM"),
                OrderId = order.OrderId,
                Method = "stripe",
                Amount = total,
                Status = paymentIntent.Status,
                ReferenceCode = paymentIntent.Id,
                PaidAt = null
            };
            await _db.Payments.AddAsync(payment);
            await _db.SaveChangesAsync();

            // 5) ส่งผลลัพธ์กลับ (client ใช้ client_secret กับ Stripe SDK)
            var result = new OrderItemDTO
            {
                OrderId = order.OrderId,
                PaymentId = payment.PaymentId,
                ClientSecret = paymentIntent.ClientSecret,
                Amount = total,
                Currency = dto.Currency,
                PaymentStatus = paymentIntent.Status
            };

            return new ResponseData(HttpStatusCode.OK, true, "สร้างคำสั่งซื้อสำเร็จ", result);
        }

        public async Task<ResponseMessage> HandleStripeWebhookAsync(HttpRequest request)
        {
            // อ่าน payload แล้วตรวจลายเซ็น
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            StripeEvent stripeEvent;

            try
            {
                var signatureHeader = request.Headers["Stripe-Signature"];
                stripeEvent = Stripe.EventUtility.ConstructEvent(json, signatureHeader, _stripeSettings.WebhookSecret);
            }
            catch (Exception ex)
            {
                return new ResponseMessage(HttpStatusCode.BadRequest, false, $"Invalid webhook: {ex.Message}");
            }

            // จัดการเฉพาะเหตุการณ์สำคัญ
            if (stripeEvent.Type == PI_SUCCEEDED)
            {
                var pi = stripeEvent.Data.Object as Stripe.PaymentIntent;
                var orderId = pi?.Metadata.GetValueOrDefault("order_id");

                if (!string.IsNullOrEmpty(orderId))
                {
                    var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                    if (order != null)
                    {
                        order.PaymentStatus = "paid";
                        order.UpdatedAt = DateTime.UtcNow;

                        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.ReferenceCode == pi.Id);
                        if (payment != null)
                        {
                            payment.Status = pi.Status;
                            payment.PaidAt = DateTime.UtcNow;
                        }

                        await _db.SaveChangesAsync();
                        await _hub.Clients.All.SendAsync("OrderChanged", new { Action = "Paid", Id = order.OrderId });
                    }
                }
            }
            else if (stripeEvent.Type == PI_FAILED)
            {
                var pi = stripeEvent.Data.Object as Stripe.PaymentIntent;
                var orderId = pi?.Metadata.GetValueOrDefault("order_id");

                if (!string.IsNullOrEmpty(orderId))
                {
                    var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                    if (order != null)
                    {
                        order.PaymentStatus = "failed";
                        order.UpdatedAt = DateTime.UtcNow;

                        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.ReferenceCode == pi.Id);
                        if (payment != null) payment.Status = pi.Status;

                        await _db.SaveChangesAsync();
                    }
                }
            }

            return new ResponseMessage(HttpStatusCode.OK, true, "ok");
        }

        private async Task<decimal> GetUnitPriceAsync(string refId)
        {
            // ลองหาจาก Ticket ก่อน ถ้าไม่มีค่อยดู Product
            var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.TicketId == refId);
            if (ticket?.Price is decimal tPrice) return tPrice;

            var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == refId);
            if (product?.Price is decimal pPrice) return pPrice;

            throw new InvalidOperationException($"ไม่พบราคาอ้างอิงของ {refId}");
        }
    }
}
