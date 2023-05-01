using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines.InfrastuctureLayer
{
  public interface IFlightRepository
  {
    Task<Flight?> GetFlightByFlightNumber(int flightNumber);
    Task<Flight[]> GetFlights();
  }
}