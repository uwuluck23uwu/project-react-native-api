namespace ProjectReactNative.Services.IServices
{
    public interface IQrScanLogService
    {
        Task<ResponseMessage> CreateAsync(string productId);
    }
}
