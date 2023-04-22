using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
  public interface IFlightRepository
  {
    Task<Flight?> GetFlightByFlightNumber(int flightNumber);
    Task<Flight[]> GetFlights();
  }
}