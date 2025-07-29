using ClassLibrary.Models.Dto;
using ClassLibrary.Models.Data;
using ProjectReactNative.Helpers;

namespace ProjectReactNative.Services.IServices
{
    public interface IHabitatService : IService<Habitat>
    {
        Task<ResponseMessage> CreateAsync(List<HabitatCreateDTO> createDTOs);
        Task<ResponseMessage> UpdateAsync(HabitatUpdateDTO updateDTO);
    }
}
