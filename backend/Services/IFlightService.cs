﻿using backend.Dtos;
using backend.Models;

namespace backend.Services
{
    public interface IFlightService
    {
        Task<List<FlightResponse>> GetAllFlights();

        Task<Flight?> GetFlightById(int id);
        Task<Flight> CreateFlight(FlightCreationRequest flightCreationRequest);
        Task<List<FlightResponse>> GetFlightsByDepartureDestinationAndDepartureDate(int departureAirportId, int destinationAirportId, DateOnly departureDate);

        Task<FlightClass?> GetFlightClassById(int id);

        Task CancelFlight(int id);

        Task ChangeFlight();
    }
}
