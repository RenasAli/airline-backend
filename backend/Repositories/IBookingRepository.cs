using backend.Dtos;
using backend.Models;

namespace backend.Repositories
{
    public interface IBookingRepository
    {
        Task<List<BookingResponse>> GetBookingsByUserId(int id);
        Task<Booking> CreateBooking(BookingProcessedRequest bookingProcessedRequest);
    }
}
