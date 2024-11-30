﻿using AutoMapper;
using backend.Dtos;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class BookingService(
        IUserService userService,
        IFlightService flightService,
        IBookingRepository bookingRepository,
        IMapper mapper,
        ITicketAvailabilityChecker ticketAvailabilityChecker
        ) : IBookingService
    {
        private readonly IUserService _userService = userService;
        private readonly IFlightService _flightService = flightService;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ITicketAvailabilityChecker _ticketAvailabilityChecker = ticketAvailabilityChecker;

        public async Task<ServiceResult<Booking>> CreateBooking(BookingCreationRequest bookingCreationRequest)
        {
            var user = await _userService.GetUserByEmail(bookingCreationRequest.Email);
            if (user == null)
            {
                return ServiceResult<Booking>.Failure($"No user found with the email: {bookingCreationRequest.Email}.");
            }

            // Map the CreationRequest DTO to ProcessedRequest DTO to allow for more properties without polluting the CreationRequest DTO
            BookingProcessedRequest bookingProcessedRequest = _mapper.Map<BookingProcessedRequest>(bookingCreationRequest);

            foreach (var ticket in bookingProcessedRequest.Tickets)
            {
                var flight = await _flightService.GetFlightById(ticket.FlightId);

                if (flight == null)
                {
                    return ServiceResult<Booking>.Failure($"No flight found with the id: {ticket.FlightId}.");
                }

                var flightClass = await _flightService.GetFlightClassById(ticket.FlightClassId);

                if (flightClass == null)
                {
                    return ServiceResult<Booking>.Failure($"No flight class found with the id: {ticket.FlightClassId}.");
                }

                ticket.FlightClassName = flightClass.Name;
                ticket.TicketNumber = GenerateUniqueString();
                ticket.FlightPrice = CalculateTicketPrice(flight, flightClass);

                
                _ticketAvailabilityChecker.AddAmountOfTicketsForFlightIdAndFlightClass(ticket.FlightId, ticket.FlightClassName);
                _ticketAvailabilityChecker.AddFlight(flight);
            }

            bool ticketsAreAvailable = _ticketAvailabilityChecker.CheckTicketAvailability();
            if (!ticketsAreAvailable)
            {
                return ServiceResult<Booking>.Failure($"Some of the tickets requested are unavailable.");
            }

            bookingProcessedRequest.ConfirmationNumber = GenerateUniqueString();
            bookingProcessedRequest.UserId = user.Id;
            var booking = await _bookingRepository.CreateBooking(bookingProcessedRequest);
            return ServiceResult<Booking>.Success(booking, "The booking was created successfully");
        }
        private decimal CalculateTicketPrice(Flight flight, FlightClass flightClass)
        {
            decimal ticketPrice = flightClass.PriceMultiplier * flight.Price;
            return ticketPrice;
        }

        private string GenerateUniqueString()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");

            int randomStringLength = 6;
            Random random = new Random();
            string stringPart = "";

            char[] characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                              'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                              '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            for (int i = 0; i < randomStringLength; i++)
            {
                int randomNumber = random.Next(0, characters.Length);
                stringPart += characters[randomNumber];
            }
            string bookingConfirmationNumber = datePart + "-" + stringPart;
            return bookingConfirmationNumber;
        }
    }
}