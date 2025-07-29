namespace ProjectReactNative.Services.IServices
{
    public interface IFacilityService : IService<Facility>
    {
        Task<ResponseMessage> CreateAsync(List<FacilityCreateDTO> createDTOs);
        Task<ResponseMessage> UpdateAsync(FacilityUpdateDTO updateDTO);
    }
}
