using FlyingDutchmanAirlines.InfrastuctureLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.InfrastuctureLayer
{
  public interface IFlyingDutchmanAirlinesContext
  {
    DbSet<Airport> Airports { get; set; }
    DbSet<Booking> Bookings { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Flight> Flights { get; set; }
  }
}