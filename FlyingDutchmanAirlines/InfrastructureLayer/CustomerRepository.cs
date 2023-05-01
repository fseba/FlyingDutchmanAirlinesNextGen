using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.InfrastuctureLayer.Models;

namespace FlyingDutchmanAirlines.InfrastuctureLayer;

public class CustomerRepository : ICustomerRepository
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

  public async Task<bool> AddCustomer(Customer customer)
  {
    if (customer is null)
    {
      return false;
    }

    try
    {
      _context.Customers.Add(customer);
      var result = await _context.SaveChangesAsync();

      return result > 0;
    }
    catch (DbUpdateException ex)
    {
      Console.WriteLine($"Error caught {ex.Message} - SaveChangesAsync failed");
      return false;
    }
  }

  public async Task<Customer?> GetCustomerByName(string name)
  {
    return Customer.IsInvalidCustomerName(name)
      ? null
      : await _context.Customers.Include(c => c.Bookings)
                                .FirstOrDefaultAsync(c => c.Name == name);
  }
}

