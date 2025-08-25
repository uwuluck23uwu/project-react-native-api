namespace ProjectReactNative.Services.IServices
{
    public interface IImageService : IService<Image>
    {
        Task<ResponseMessage> CreateAsync(IFormFile file, string refId);
        Task<ResponseMessage> UpdateAsync(IFormFile file, string imageUrl);
        Task<ResponseMessage> DeleteAsync(List<string> Ids, string refId);
    }
}
