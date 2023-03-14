using System;
namespace FlyingDutchmanAirlines.Exceptions
{
  public class AirportNotFoundException : Exception
  {
    public AirportNotFoundException() {}

    public AirportNotFoundException(string message, Exception innerException)
    : base(message, innerException) { }
  }
}

