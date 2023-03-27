using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class BookingRepository
{
  private readonly FlyingDutchmanAirlinesContext _context = null!;

  public BookingRepository(FlyingDutchmanAirlinesContext injectedContext)
  {
    _context = injectedContext;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public BookingRepository()
  {
    if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
    {
      throw new InvalidOperationException("This constructor should only be used for testing");
    }
  }

  public virtual async Task CreateBooking(int customerId, int flightNumber)
  {
    if (int.IsNegative(customerId) || int.IsNegative(flightNumber))
    {
      Console.WriteLine($"Argument Exception in CreatingBooking! " +
        $"Customer ID = {customerId}, flightNumber = {flightNumber}");
      throw new ArgumentException("Invalid arguments provided");
    }

    var newBooking = new Booking() 
    {
      CustomerId = customerId,
      FlightNumber = flightNumber
    };

    try
    {
      _context.Bookings.Add(newBooking);
      await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Exception during database query: {ex.Message}");
      throw new CouldNotAddBookingToDatabaseException($"Exception during database query: {ex.Message}", ex.InnerException!);
    }
  }

  public virtual async Task<Booking?> GetBookingById(int bookingId)
  {
    if (!await _context.Bookings.AnyAsync())
    {
      Console.WriteLine("No bookings in database!");
      return null;
    }

    return await _context.Bookings.Include("Customer")
                                  .FirstOrDefaultAsync(b => b.BookingId == bookingId);
  }

  public virtual async Task<Booking?> DeleteBooking(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      Console.WriteLine($"Argument Exception in DeleteBooking! " +
        $"Booking ID = {bookingId}");
      throw new ArgumentException("Invalid argument provided");
    }

    var booking = await GetBookingById(bookingId);

    if (booking is null)
    {
      return null; 
    }

    var deletedBooking = _context.Bookings.Remove(booking).Entity;
    await _context.SaveChangesAsync();

    return deletedBooking;
  }
}

