namespace ProjectReactNative.Services.IServices
{
    public interface ILocationService : IService<Location>
    {
        Task<ResponsePagination> GetAllLocations(int pageSize, int currentPage, string search);
        Task<ResponseMessage> CreateAsync(LocationCreateDTO createDTO);
        Task<ResponseMessage> UpdateAsync(LocationUpdateDTO updateDTO);
    }
}
