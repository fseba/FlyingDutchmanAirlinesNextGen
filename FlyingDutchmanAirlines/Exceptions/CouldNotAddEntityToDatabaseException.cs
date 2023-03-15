namespace FlyingDutchmanAirlines.Exceptions;

public class CouldNotAddEntityToDatabaseException : Exception
{
  public CouldNotAddEntityToDatabaseException()
  {
  }

  public CouldNotAddEntityToDatabaseException(string? message, Exception? innerException) : base(message, innerException)
  {
  }
}

