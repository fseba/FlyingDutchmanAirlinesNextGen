using FlyingDutchmanAirlines.Views;

namespace FlyingDutchmanAirlines.ServiceLayer
{
  public interface IFlightService
  {
    Task<FlightView?> GetFlightByFlightNumber(int flightNumber);
    IAsyncEnumerable<FlightView> GetFlights();
  }
}