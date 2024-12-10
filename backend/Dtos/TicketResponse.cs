public class TicketResponse {

//    public int Id {get; set;}

    public decimal Price {get; set;}
    public string TicketNumber {get; set;}
    public string FlightCode { get; set; } = null!;
    public string DeparturePortName { get; set; } = null!;
    public string ArrivalPortName { get; set; } = null!;
    public string FlightClassName { get; set; } = null!;
    public string PassengerFirstName { get; set; } = null!;
    public string PassengerLastName { get; set; } = null!;
    public string PassengerEmail { get; set; } = null!;
}