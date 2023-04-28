using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines.DTOs;

public class BookingDTO
{
  public int BookingId { get; private set; }
  public int CustomerId { get; private set; }
  public string CustomerName { get; private set; }
  public FlightDTO FlightView { get; private set; }

  public BookingDTO(Booking booking, FlightDTO flightView)
  {
    if (booking is null || flightView is null)
    {
      throw new ArgumentNullException();
    }

    BookingId = booking.BookingId;
    CustomerId = booking.CustomerId!.Value;
    CustomerName = booking.Customer!.Name;
    FlightView = flightView;
  }
}

