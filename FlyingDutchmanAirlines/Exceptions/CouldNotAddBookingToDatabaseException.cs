using System;
namespace FlyingDutchmanAirlines.Exceptions;

public class CouldNotAddBookingToDatabaseException : CouldNotAddEntityToDatabaseException
{
  public CouldNotAddBookingToDatabaseException()
  {
  }

  public CouldNotAddBookingToDatabaseException(string message, Exception innerException)
    :base(message, innerException) {  }
}

