namespace FlyingDutchmanAirlines.Views;

public class BookingView
{
  public int BookingId { get; private set; }
  public int CustomerId { get; private set; }
  public string CustomerName { get; private set; }
  public FlightView FlightView { get; private set; }

  public BookingView(int bookingId, int customerId, string customerName, FlightView flightView)
  {
    BookingId = bookingId;
    CustomerId = customerId;
    CustomerName = customerName;
    FlightView = flightView;
  }
}

