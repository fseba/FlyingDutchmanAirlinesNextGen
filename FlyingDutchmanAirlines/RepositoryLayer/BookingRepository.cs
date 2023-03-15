using System.Reflection;
using System.Runtime.CompilerServices;

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

  public virtual async Task CreateBooking(int customerID, int flightNumber)
  {
    if (int.IsNegative(customerID) || int.IsNegative(flightNumber))
    {
      Console.WriteLine($"Argument Exception in CreatingBooking! " +
        $"Customer ID = {customerID}, flightNumber = {flightNumber}");
      throw new ArgumentException("Invalid arguments provided");
    }

    Booking newBooking = new() 
    {
      CustomerId = customerID,
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
      throw new CouldNotAddBookingToDatabaseException($"Exception during database query: {ex.Message}", ex);
    }
  }
}

