using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
  public interface IBookingRepository
  {
    Task<bool> CreateBooking(int customerId, int flightNumber);
    Task<bool> DeleteBooking(int bookingId);
    Task<Booking?> GetBookingById(int bookingId);
  }
}