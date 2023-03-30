using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;


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

  public virtual async Task<bool> CreateBooking(int customerId, int flightNumber)
  {
    var newBooking = Booking.Create(customerId, flightNumber); 

    try
    {
      _context.Bookings.Add(newBooking);
      await _context.SaveChangesAsync();
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Exception during database query: {ex.Message}");
      return false;
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

  public virtual async Task<bool> DeleteBooking(int bookingId)
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
      return false; 
    }

    _context.Bookings.Remove(booking);
    await _context.SaveChangesAsync();

    return true;
  }
}

