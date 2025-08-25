using System.Linq.Expressions;

namespace ProjectReactNative.Services.IServices
{
    public interface IService<T> where T : class
    {
        Task<ResponsePagination> GetAllAsync(int pageSize, int currentPage, string search, Expression<Func<T, object>>[]? includes);
        Task<ResponseData> GetAsync(Expression<Func<T, bool>> id);
        Task<ResponseMessage> CreateAsync(T model);
        Task<ResponseMessage> UpdateAsync(T model);
        Task<ResponseMessage> DeleteAsync(IEnumerable<string> ids);
        Task<ResponseMessage> DeleteImagesAsync(IEnumerable<string> ids);
        Task<ResponseMessage> DeleteLocationAsync(IEnumerable<string> ids);
    }
}
