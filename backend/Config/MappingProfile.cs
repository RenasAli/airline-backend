using AutoMapper;
using backend.Dtos;
using backend.Models;
using backend.Models.MongoDB;

namespace backend.Config
{
	public class MappingProfile: Profile
	{

		public MappingProfile() {
			CreateMap<User, UserResponse>();

			CreateMap<FlightCreationRequest, Flight>()
			   .ForMember(dest => dest.FlightsAirlineId, opt => opt.MapFrom(src => src.AirlineId))
			   .ForMember(dest => dest.FlightsAirplaneId, opt => opt.MapFrom(src => src.AirplaneId))
			   .ForMember(dest => dest.DeparturePort, opt => opt.MapFrom(src => src.DepartureAirportId))
			   .ForMember(dest => dest.ArrivalPort, opt => opt.MapFrom(src => src.ArrivalAirportId))
			   .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureDateTime));
			
			CreateMap<Flight, FlightResponse>();

			CreateMap<User, JwtRequest>();

            CreateMap<Airport, AirportResponse>();

            CreateMap<Airplane, AirplaneResponse>();
            
            CreateMap<Airline, AirlineResponse>();

            CreateMap<BookingCreationRequest, BookingProcessedRequest>();

            CreateMap<TicketCreationRequest, TicketProcessedRequest>();

			CreateMap<Booking, BookingResponse>();

			// Mappings from MongoDB entities to the "shared" models
			CreateMap<AirplaneMongo, Airplane>();

            CreateMap<AirlineSnapshot, Airline>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<AirplaneSnapshot, Airplane>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.EconomyClassSeats, opt => opt.MapFrom(src => src.EconomyClassSeats))
               .ForMember(dest => dest.BusinessClassSeats, opt => opt.MapFrom(src => src.BusinessClassSeats))
               .ForMember(dest => dest.FirstClassSeats, opt => opt.MapFrom(src => src.FirstClassSeats));

            // Additional mapping for AirportSnapshot -> Airport
            CreateMap<AirportSnapshot, Airport>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.City.Id))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

            CreateMap<CitySnapshot, City>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.State.Id))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State));

            CreateMap<StateSnapshot, State>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code));


            CreateMap<FlightMongo, Flight>()
             .ForMember(dest => dest.FlightsAirlineId, opt => opt.MapFrom(src => src.FlightsAirline.Id))
             .ForMember(dest => dest.FlightsAirplaneId, opt => opt.MapFrom(src => src.FlightsAirplane.Id))
             .ForMember(dest => dest.ArrivalPort, opt => opt.MapFrom(src => src.ArrivalPort.Id))
             .ForMember(dest => dest.DeparturePort, opt => opt.MapFrom(src => src.DeparturePort.Id))
             .ForMember(dest => dest.ArrivalPortNavigation, opt => opt.MapFrom(src => src.ArrivalPort))
             .ForMember(dest => dest.DeparturePortNavigation, opt => opt.MapFrom(src => src.DeparturePort));
        }
    }
}
