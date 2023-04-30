using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines.DTOs;

public class BookingDTO
{
  public int BookingId { get; private set; }
  public int CustomerId { get; private set; }
  public string CustomerName { get; private set; }
  public FlightDTO FlightDTO { get; private set; }

  public BookingDTO(Booking booking, FlightDTO flightDTO)
  {
    if (booking is null || flightDTO is null)
    {
      throw new ArgumentNullException("Booking or FlightDTO is null");
    }

    if (booking.Customer is null || booking.CustomerId is null)
    {
      throw new ArgumentNullException("Customer information missing - Booking objects should be loaded from the database.");
    }

    BookingId = booking.BookingId;
    CustomerId = booking.CustomerId.Value;
    CustomerName = booking.Customer.Name;
    FlightDTO = flightDTO;
  }
}

