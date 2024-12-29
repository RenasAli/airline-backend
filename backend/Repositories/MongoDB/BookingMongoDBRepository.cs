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
                    ConfirmationNumber = request.ConfirmationNumber,
                    User = new UserSnapshot
                    {
                        Id = request.UserId,
                        Email = request.Email
                    },
                    Tickets = new List<TicketEmbedded>()
                };

                // Get the flight for the tickets
                var flight = await _context.Flights
                    .FirstOrDefaultAsync(f => f.Id == request.Tickets.First().FlightId);

                // Get all tickets from the booking
                /*var tickets = await _context.Bookings
                    .Where(b => request.Tickets.Select(t => t.TicketNumber).Contains(b.ConfirmationNumber))
                    .ToListAsync();*/

                // Add tickets to the BookingMongo
                foreach (var ticket in request.Tickets)
                {
                    var passenger = new PassengerEmbedded
                    {
                        Id = UniqueSequenceGenerator.GenerateLongIdUsingTicks(),
                        FirstName = ticket.Passenger.FirstName,
                        LastName = ticket.Passenger.LastName,
                        Email = ticket.Passenger.Email
                    };

                    var flightSnap = new FlightSnapShot
                    {
                        Id = flight.Id,
                        FlightCode = flight.FlightCode,
                        DepartureTime = flight.DepartureTime,
                        CompletionTime = flight.CompletionTime
                    };

                    var flightClass = new FlightClassSnapshot
                    {
                        Id = ticket.FlightClassId,
                        Name = ticket.FlightClassName,
                        PriceMultiplier = ticket.FlightPrice // ???

                    };

                    var ticketEmbedded = new TicketEmbedded
                    {
                        Id = UniqueSequenceGenerator.GenerateLongIdUsingTicks(),
                        TicketNumber = ticket.TicketNumber,
                        Flight = flightSnap,
                        Passenger = passenger,
                        FlightClass = flightClass
                    };

                    bookingMongo.Tickets.Add(ticketEmbedded);
                }

                // Add and save the BookingMongo to the MongoDB context
                _context.Bookings.Add(bookingMongo);
                await _context.SaveChangesAsync();

                // Map BookingMongo to Booking for the return value
                var booking = new Booking
                {
                    Id = bookingMongo.Id,
                    ConfirmationNumber = bookingMongo.ConfirmationNumber,
                    UserId = bookingMongo.User.Id,
                    Tickets = bookingMongo.Tickets.Select(t => new Ticket
                    {
                        Id = t.Id,
                        Price = t.Price,
                        TicketNumber = t.TicketNumber,
                        Passenger = new Passenger
                        {
                            Id = t.Passenger.Id,
                            FirstName = t.Passenger.FirstName,
                            LastName = t.Passenger.LastName,
                            Email = t.Passenger.Email
                        },
                        Flight = new Flight
                        {
                            Id = t.Flight.Id,
                            FlightCode = t.Flight.FlightCode,
                            DepartureTime = t.Flight.DepartureTime,
                            CompletionTime = t.Flight.CompletionTime,
                        },
                        FlightClass = new FlightClass
                        {
                            Id = t.FlightClass.Id,
                            Name = t.FlightClass.Name,
                            PriceMultiplier = t.FlightClass.PriceMultiplier
                        }
                    }).ToList()
                };

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
