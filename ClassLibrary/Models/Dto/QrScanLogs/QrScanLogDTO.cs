#nullable disable

namespace ClassLibrary.Models.Dto.QrScanLogs
{
    public class QrScanLogDTO
    {
        public string ScanId { get; set; }

        public string ProductId { get; set; }

        public string QrCodeUrl { get; set; }

        public DateTime? ScannedAt { get; set; }
    }
}
