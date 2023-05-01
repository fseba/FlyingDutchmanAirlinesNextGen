using FlyingDutchmanAirlines.DTOs;

namespace FlyingDutchmanAirlines.BusinessLogicLayer;

public interface IBookingService
{
  Task<bool> CreateBooking(string customerName, int flightNumber);
  Task<bool> DeleteBooking(int bookingId);
  Task<BookingDTO?> GetBookingById(int bookingId);
  IAsyncEnumerable<BookingDTO?> GetBookingsByCustomerName(string customerName);
}