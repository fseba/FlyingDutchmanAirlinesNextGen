using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines.InfrastuctureLayer
{
  public interface ICustomerRepository
  {
    Task<bool> AddCustomer(Customer customer);
    Task<Customer?> GetCustomerByName(string name);
  }
}