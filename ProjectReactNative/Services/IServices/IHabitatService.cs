namespace ProjectReactNative.Services.IServices
{
    public interface IHabitatService : IService<Habitat>
    {
        Task<ResponsePagination> GetAllHabitats(int pageSize, int currentPage, string search);
        Task<ResponseMessage> CreateAsync(List<HabitatCreateDTO> createDTOs);
        Task<ResponseMessage> UpdateAsync(HabitatUpdateDTO updateDTO);
    }
}
