using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;

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
      throw new Exception("This constructor should only be used for testing");
    }
  }

  public virtual async Task<bool> CreateCustomer(string name)
  {
    if (IsInvalidCustomerName(name))
    {
      return false;
    }

    try
    {
      Customer newCustomer = new(name);

      _context.Customers.Add(newCustomer);
      await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error caught {ex.Message} - SaveChangesAsync failed");
      return false;
    }

    return true;
  }

  public virtual async Task<Customer> GetCustomerByName(string name)
  {
    if (IsInvalidCustomerName(name))
    {
      throw new CustomerNotFoundException();
    }

    return await _context.Customers.Include("Bookings")
                                   .FirstOrDefaultAsync(c => c.Name == name)
           ?? throw new CustomerNotFoundException();
  }

  private static bool IsInvalidCustomerName(string name)
  {
    char[] forbiddenCharacters = { '!', '@', '#', '$', '%', '&', '*', '/', '=' };
    return string.IsNullOrWhiteSpace(name) || name.Any(c => forbiddenCharacters.Contains(c));
  }
}

