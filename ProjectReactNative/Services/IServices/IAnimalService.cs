namespace ProjectReactNative.Services.IServices
{
    public interface IAnimalService : IService<Animal>
    {
        Task<ResponsePagination> GetAllAnimals(int pageSize, int currentPage, string search);
        Task<ResponseMessage> CreateAsync(List<AnimalCreateDTO> createDTOs);
        Task<ResponseMessage> UpdateAsync(AnimalUpdateDTO updateDTO);
    }
}
