namespace ProjectReactNative.Services.IServices
{
    public interface ITicketService : IService<Ticket>
    {
        Task<ResponsePagination> GetAllTickets(int pageSize, int currentPage, string search);
        Task<ResponseMessage> CreateAsync(List<TicketCreateDTO> createDTOs);
        Task<ResponseMessage> UpdateAsync(TicketUpdateDTO updateDTO);
    }
}
