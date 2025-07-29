namespace ProjectReactNative.Services.IServices
{
    public interface IEventService : IService<Event>
    {
        Task<ResponsePagination> GetAllEvents(int pageSize, int currentPage, string search);
        Task<ResponseMessage> CreateAsync(List<EventCreateDTO> createDTOs);
        Task<ResponseMessage> UpdateAsync(EventUpdateDTO updateDTO);
    }
}
