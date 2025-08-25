using AutoMapper;
using ClassLibrary.Models.Data;

namespace ProjectReactNative.Helpers.Configures
{
    public class MappingConfigure : Profile
    {
        public MappingConfigure()
        {
            AddMappings<Animal, AnimalDTO, AnimalCreateDTO, AnimalUpdateDTO>();
            AddMappings<Event, EventDTO, EventCreateDTO, EventUpdateDTO>();
            //AddMappings<EventStaff, EventStaffDTO, EventStaffCreateDTO, EventStaffUpdateDTO>();
            AddMappings<Habitat, HabitatDTO, HabitatCreateDTO, HabitatUpdateDTO>();
            //AddMappings<Image, ImageDTO, ImageCreateDTO, ImageUpdateDTO>();
            AddMappings<Location, LocationDTO, LocationCreateDTO, LocationUpdateDTO>();
            AddMappings<News, NewsDTO, NewsCreateDTO, NewsUpdateDTO>();
            AddMappings<OrderItem, OrderItemDTO, OrderItemCreateDTO, OrderItemUpdateDTO>();
            AddMappings<Order, OrderDTO, OrderCreateDTO, OrderUpdateDTO>();
            AddMappings<Payment, PaymentDTO, PaymentCreateDTO, PaymentUpdateDTO>();
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
            CreateMap<TCreateDTO, TUpdateDTO>().ReverseMap();
        }
    }
}
