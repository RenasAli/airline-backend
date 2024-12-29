using AutoMapper;
using backend.Database;
using backend.Dtos;
using backend.Models;
using backend.Models.MongoDB;
using Microsoft.EntityFrameworkCore;
using backend.Utils;

namespace backend.Repositories.MongoDB
{
    public class BookingMongoDBRepository(MongoDBContext context, IMapper mapper): IBookingRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Booking> CreateBooking(BookingProcessedRequest request)
        {
            try
            {
                // Create the BookingMongo document
                var bookingMongo = new BookingMongo
                {
                    Id = UniqueSequenceGenerator.GenerateLongIdUsingTicks(),
                    ConfirmationNumber = request.ConfirmationNumber,
                    User = new UserSnapshot
                    {
                        Id = request.UserId,
                        Email = request.Email
                    },
                    Tickets = new List<TicketEmbedded>()
                };

                // Add tickets to the BookingMongo
                foreach (var ticket in request.Tickets)
                {
                    // Fetch the flight for the current ticket
                    var flight = await _context.Flights
                        .FirstOrDefaultAsync(f => f.Id == ticket.FlightId);

                    if (flight == null)
                    {
                        throw new Exception($"Flight with ID {ticket.FlightId} not found.");
                    }

                    // Create a new FlightSnapShot instance for each ticket
                    var flightSnap = new FlightSnapShot
                    {
                        Id = flight.Id,
                        FlightCode = flight.FlightCode,
                        DepartureTime = flight.DepartureTime,
                        CompletionTime = flight.CompletionTime,
                        TravelTime = flight.TravelTime,
                        Kilometers = flight.Kilometers,
                        Price = flight.Price,
                        EconomyClassSeatsAvailable = flight.EconomyClassSeatsAvailable,
                        BusinessClassSeatsAvailable = flight.BusinessClassSeatsAvailable,
                        FirstClassSeatsAvailable = flight.FirstClassSeatsAvailable,
                        ArrivalPort = flight.ArrivalPort,
                        DeparturePort = flight.DeparturePort,
                        FlightsAirline = flight.FlightsAirline,
                        FlightsAirplane = flight.FlightsAirplane
                    };

                    // Get flight class for the ticket by flight class id
                    var flightClass = await _context.FlightClasses
                        .FirstOrDefaultAsync(fc => fc.Id == ticket.FlightClassId);

                    if (flightClass == null)
                    {
                        throw new Exception($"Flight class with ID {ticket.FlightClassId} not found.");
                    }

                    var flightClassSnap = new FlightClassSnapshot
                    {
                        Id = ticket.FlightClassId,
                        Name = ticket.FlightClassName,
                        PriceMultiplier = flightClass.PriceMultiplier
                    };

                    var passenger = new PassengerEmbedded
                    {
                        Id = UniqueSequenceGenerator.GenerateLongIdUsingTicks(),
                        FirstName = ticket.Passenger.FirstName,
                        LastName = ticket.Passenger.LastName,
                        Email = ticket.Passenger.Email
                    };

                    // Create and add the ticket
                    var ticketEmbedded = new TicketEmbedded
                    {
                        Id = UniqueSequenceGenerator.GenerateLongIdUsingTicks(),
                        TicketNumber = ticket.TicketNumber,
                        Price = ticket.FlightPrice,
                        Flight = flightSnap,
                        Passenger = passenger,
                        FlightClass = flightClassSnap
                    };

                    bookingMongo.Tickets.Add(ticketEmbedded);
                }

                // Add and save the BookingMongo to the MongoDB context
                _context.Bookings.Add(bookingMongo);
                await _context.SaveChangesAsync();

                // Map BookingMongo to Booking for the return value
                var booking = _mapper.Map<Booking>(bookingMongo);

                return booking;
            }
            catch (Exception ex)
            {
                // Handle or log exceptions as needed
                throw new ApplicationException("An error occurred while creating the booking.", ex);
            }
        }


        public async Task<List<Booking>> GetBookingsByUserId(long id)
        {
            var bookings = await _context.Bookings.Where(booking => booking.User.Id == id)
                .ToListAsync();
            return _mapper.Map<List<Booking>>(bookings);
        }
    }


}
