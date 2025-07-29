namespace ProjectReactNative.Services.IServices
{
    public interface INewsService : IService<News>
    {
        Task<ResponsePagination> GetAllNews(int pageSize, int currentPage, string search);
        Task<ResponseMessage> CreateAsync(List<NewsCreateDTO> createDTOs);
        Task<ResponseMessage> UpdateAsync(NewsUpdateDTO updateDTO);
    }
}
