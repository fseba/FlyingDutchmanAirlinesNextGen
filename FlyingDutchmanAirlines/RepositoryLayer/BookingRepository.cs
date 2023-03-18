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
      throw new Exception("This constructor should only be used for testing");
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

    Booking newBooking = new() 
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

  public async Task<Booking> GetBookingById(int bookingId)
  {
    try
    {
      if (!await _context.Bookings.AnyAsync())
      {
        Console.WriteLine("No bookings in database!");
        throw new BookingNotFoundException();
      }

      return await _context.Bookings.FindAsync(bookingId)
          ?? throw new BookingNotFoundException();
    }
    catch (Exception)
    {
      throw;
    }
  }

  public async Task<Booking[]> GetBookingsByCustomerId(int customerId)
  {
    try
    {
      if (!await _context.Bookings.AnyAsync())
      {
        Console.WriteLine("No bookings in database!");
        throw new BookingNotFoundException();
      }

      Booking[] bookings = await _context.Bookings.Where(b => b.CustomerId == customerId)
                                                  .ToArrayAsync();

      return bookings.Length == 0
        ? throw new BookingNotFoundException()
        : bookings;
    }
    catch (Exception)
    {
      throw;
    }
  }

  public virtual async Task DeleteBooking(int bookingId)
  {
    if (int.IsNegative(bookingId))
    {
      Console.WriteLine($"Argument Exception in DeleteBooking! " +
        $"Booking ID = {bookingId}");
      throw new ArgumentException("Invalid argument provided");
    }

    try
    {
      Booking booking = await GetBookingById(bookingId);

      _context.Bookings.Remove(booking);
      await _context.SaveChangesAsync();
    }
    catch (Exception)
    {
      throw;
    }
  }
}

