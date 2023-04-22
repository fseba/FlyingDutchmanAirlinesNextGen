using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ServiceLayer
{
  public interface IBookingService
  {
    Task<bool> CreateBooking(string customerName, int flightNumber);
    Task<bool> DeleteBooking(int bookingId);
    Task<BookingView?> GetBookingById(int bookingId);
    IAsyncEnumerable<BookingView?> GetBookingsByCustomerName(string customerName);
  }
}