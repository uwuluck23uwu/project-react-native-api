namespace ProjectReactNative.Services.IServices
{
    public interface IImageService : IService<Image>
    {
        Task<ResponseMessage> CreateAsync(IFormFile file, string refId);
        Task<ResponseMessage> DeleteAsync(List<string> Ids, string refId);
    }
}
