using Microsoft.AspNetCore.SignalR;
using QRCoder;
using System.Net;

namespace ProjectReactNative.Services
{
    public class QrScanLogService : Service<QrScanLog>, IQrScanLogService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public QrScanLogService(
            ApplicationDbContext db,
            IHubContext<SignalHub> hub,
            IWebHostEnvironment hostEnvironment
        ) : base(db, hub)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ResponseMessage> CreateAsync(string scanId)
        {
            var product = await _db.Products.FindAsync(scanId);
            if (product == null)
            {
                return new ResponseMessage(HttpStatusCode.NotFound, false, "ไม่พบสินค้า");
            }

            var smartUrl = $"http://localhost:5173/product/{scanId}";
            var qrCodeUrl = await GenerateQrCodeImageAsync(smartUrl);

            var model = new QrScanLog
            {
                ScanId = await GenerateRunningIdAsync("ScanId", "SC"),
                ProductId = scanId,
                QrCodeUrl = qrCodeUrl,
                ScannedAt = DateTime.UtcNow
            };

            await CreateAsync(model);

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้าง QR Code สำเร็จ"
            );
        }

        public async Task<string> GenerateQrCodeImageAsync(string content)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);

            var uploadsRoot = Path.Combine(_hostEnvironment.WebRootPath, "qrcode");
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}.png";
            var filePath = Path.Combine(uploadsRoot, fileName);
            await File.WriteAllBytesAsync(filePath, qrBytes);

            return $"/qrcode/{fileName}";
        }
    }
}
