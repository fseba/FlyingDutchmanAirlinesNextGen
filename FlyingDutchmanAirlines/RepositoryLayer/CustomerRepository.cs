using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class CustomerRepository
{
  private readonly FlyingDutchmanAirlinesContext _context = null!;

  public CustomerRepository(FlyingDutchmanAirlinesContext injectedContext)
  {
    _context = injectedContext;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public CustomerRepository()
  {
    if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
    {
      throw new InvalidOperationException("This constructor should only be used for testing");
    }
  }

  public virtual async Task<bool> AddCustomer(Customer customer)
  {
    if (customer is null)
    {
      return false;
    }

    try
    {
      _context.Customers.Add(customer);
      await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error caught {ex.Message} - SaveChangesAsync failed");
      return false;
    }

    return true;
  }

  public virtual async Task<Customer?> GetCustomerByName(string name)
  {
    return Customer.IsInvalidCustomerName(name)
      ? null
      : await _context.Customers.Include("Bookings")
                                .FirstOrDefaultAsync(c => c.Name == name);
  }
}

