namespace FlyingDutchmanAirlines.Exceptions;

public class BookingNotFoundException : Exception
{
  public BookingNotFoundException() {  }

  public BookingNotFoundException(string? message) : base(message) {  }

  public BookingNotFoundException(string message, Exception innerException)
    : base(message, innerException) { }
}

