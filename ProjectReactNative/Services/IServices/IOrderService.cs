namespace ProjectReactNative.Services.IServices
{
    public interface IOrderService : IService<Order>
    {
        Task<ResponsePagination> GetAllOrders(int pageSize, int currentPage, string search);
        Task<ResponseData> CreateOrderAndPaymentAsync(OrderCreateDTO dto);
        Task<ResponseMessage> HandleStripeWebhookAsync(HttpRequest request);
    }
}
