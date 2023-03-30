using System.Security.Cryptography;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public sealed class Customer
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
  public int CustomerId { get; private set; }

  public string Name { get; private set; } = null!;

  public ICollection<Booking> Bookings { get; } = new List<Booking>();

  public static Customer? Create(string name)
  {
    if (IsInvalidCustomerName(name))
    {
      return null; 
    }

    return new Customer { Name = name };
  }

  public static bool IsInvalidCustomerName(string name)
  {
    char[] forbiddenCharacters = { '!', '@', '#', '$', '%', '&', '*', '/', '=' };
    return string.IsNullOrWhiteSpace(name) || name.Any(c => forbiddenCharacters.Contains(c));
  }

  public static bool operator == (Customer x, Customer y)
  {
    CustomerEqualityComparer comparer = new();
    return comparer.Equals(x, y);
  }

  public static bool operator != (Customer x, Customer y) => !(x == y);
}

internal class CustomerEqualityComparer : EqualityComparer<Customer>
{
  public override int GetHashCode(Customer obj)
  {
    int randomNumber = RandomNumberGenerator.GetInt32(int.MaxValue / 2);
    return (obj.CustomerId + obj.Name.Length + randomNumber).GetHashCode();
  }

  public override bool Equals(Customer? x, Customer? y)
  {
    return x!.CustomerId == y!.CustomerId && x.Name == y.Name;
  }
}