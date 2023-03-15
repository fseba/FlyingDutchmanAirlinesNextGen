namespace FlyingDutchmanAirlines.Exceptions
{
  public class FlightNotFoundException : Exception
  {
    public FlightNotFoundException() {}

    public FlightNotFoundException(string message, Exception innerException)
    : base(message, innerException) { }
  }
}

