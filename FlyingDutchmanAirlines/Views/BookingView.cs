using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines.Views;

public class BookingView
{
  public int BookingId { get; private set; }
  public int CustomerId { get; private set; }
  public string CustomerName { get; private set; }
  public FlightView FlightView { get; private set; }

  public BookingView(Booking booking, FlightView flightView)
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

