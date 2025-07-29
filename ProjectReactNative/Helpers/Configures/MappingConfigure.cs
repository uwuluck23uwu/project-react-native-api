using AutoMapper;
using ClassLibrary.Models.Dto;

namespace ProjectReactNative.Helpers.Configures
{
    public class MappingConfigure : Profile
    {
        public MappingConfigure()
        {
            AddMappings<Animal, AnimalDTO, AnimalCreateDTO, AnimalUpdateDTO>();
            AddMappings<Event, EventDTO, EventCreateDTO, EventUpdateDTO>();
            //AddMappings<EventStaff, EventStaffDTO, EventStaffCreateDTO, EventStaffUpdateDTO>();
            AddMappings<Facility, FacilityDTO, FacilityCreateDTO, FacilityUpdateDTO>();
            AddMappings<Habitat, HabitatDTO, HabitatCreateDTO, HabitatUpdateDTO>();
            //AddMappings<Image, ImageDTO, ImageCreateDTO, ImageUpdateDTO>();
            AddMappings<News, NewsDTO, NewsCreateDTO, NewsUpdateDTO>();
            //AddMappings<Order, OrderDTO, OrderCreateDTO, OrderUpdateDTO>();
            //AddMappings<OrderItem, OrderItemDTO, OrderItemCreateDTO, OrderItemUpdateDTO>();
            AddMappings<Product, ProductDTO, ProductCreateDTO, ProductUpdateDTO>();
            //AddMappings<QrScanLog, QrScanLogDTO, QrScanLogCreateDTO, QrScanLogUpdateDTO>();
            //AddMappings<RefreshToken, RefreshTokenDTO, RefreshTokenCreateDTO, RefreshTokenUpdateDTO>();
            //AddMappings<Staff, StaffDTO, StaffCreateDTO, StaffUpdateDTO>();
            AddMappings<Ticket, TicketDTO, TicketCreateDTO, TicketUpdateDTO>();
            AddMappings<User, UserDTO, RegisterationRequestDTO, LoginRequestDTO>();
            //AddMappings<UserTicket, UserTicketDTO, UserTicketCreateDTO, UserTicketUpdateDTO>();
        }

        private void AddMappings<TModel, TDTO, TCreateDTO, TUpdateDTO>()
        {
            CreateMap<TModel, TDTO>().ReverseMap();
            CreateMap<TModel, TCreateDTO>().ReverseMap();
            CreateMap<TModel, TUpdateDTO>().ReverseMap();
        }
    }
}
