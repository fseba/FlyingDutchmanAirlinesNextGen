using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines.RepositoryLayer
{
  public interface ICustomerRepository
  {
    Task<bool> AddCustomer(Customer customer);
    Task<Customer?> GetCustomerByName(string name);
  }
}