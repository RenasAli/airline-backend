﻿using backend.Models;

namespace backend.Dtos
{
    public class FlightResponse
    {
        public int Id { get; set; }

        public string FlightCode { get; set; } = null!;


        public DateTime DepartureTime { get; set; }

        public int TravelTime { get; set; }

        public string? Kilometers { get; set; }

        public double Price { get; set; }

        public virtual Airport ArrivalPortNavigation { get; set; } = null!;

        public virtual Airport DeparturePortNavigation { get; set; } = null!;

        public virtual Airline FlightsAirline { get; set; } = null!;

        public virtual Airplane FlightsAirplane { get; set; } = null!;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
