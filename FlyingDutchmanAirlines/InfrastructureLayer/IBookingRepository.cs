using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines.InfrastuctureLayer
{
  public interface IBookingRepository
  {
    Task<bool> CreateBooking(int customerId, int flightNumber);
    Task<bool> DeleteBooking(int bookingId);
    Task<Booking?> GetBookingById(int bookingId);
    Task<bool> AddBooking(Booking booking);
  }
}