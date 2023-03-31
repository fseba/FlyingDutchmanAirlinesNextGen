namespace FlyingDutchmanAirlines.DatabaseLayer.Models;

public sealed class Booking
{
  public int BookingId { get; private set; }

  public int FlightNumber { get; private set; }

  public int? CustomerId { get; private set; }

  public Customer? Customer { get; private set; }

  public Flight FlightNumberNavigation { get; private set; } = null!;

  public static Booking Create(int customerId, int flightNumber)
  {
    if (customerId < 0 || flightNumber < 0)
    {
      throw new ArgumentException("Invalid arguments provided");
    }

    return new Booking { CustomerId = customerId, FlightNumber = flightNumber };
  }
}
