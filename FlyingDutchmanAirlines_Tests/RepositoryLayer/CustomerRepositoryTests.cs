using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;

[TestClass]
public class CustomerRepositoryTests
{
  private FlyingDutchmanAirlinesContext _context = null!;
  private CustomerRepository _repository = null!; 

  [TestInitialize]
  public async Task TestInitialize()
  {
    DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions =
      new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>()
      .UseInMemoryDatabase("FlyingDutchman").Options;

    _context = new FlyingDutchmanAirlinesContext(dbContextOptions);

    Customer testCustomer = new("Linus Torvalds");
    _context.Customers.Add(testCustomer);
    await _context.SaveChangesAsync();

    _repository = new CustomerRepository(_context);
    Assert.IsNotNull(_repository);
  }

  [TestMethod]
  public async Task CreateCustomer_Success()
  {
    bool result = await _repository.CreateCustomer("Donald Knuth");
    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task CreateCustomer_Failure_NameIsNull()
  {
    bool result = await _repository.CreateCustomer(null!);
    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task CreateCustomer_Failure_NameIsEmptyString()
  {
    bool result = await _repository.CreateCustomer(string.Empty);
    Assert.IsFalse(result);
  }

  [TestMethod]
  [DataRow('!')]
  [DataRow('@')]
  [DataRow('#')]
  [DataRow('$')]
  [DataRow('%')]
  [DataRow('&')]
  [DataRow('*')]
  [DataRow('/')]
  [DataRow('=')]
  public async Task CreateCustomer_Failure_NameContainsInvalidCharacters(char invalidCharacter)
  {
    bool result = await _repository.CreateCustomer($"Donald Knuth{invalidCharacter}");
    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task CreateCustomer_Failure_DatabaseAccessError()
  {
    CustomerRepository repository = new(null!);
    Assert.IsNotNull(repository);

    bool result = await repository.CreateCustomer("Donald Knuth");
    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task GetCustomerByName_Success()
  {
    var customer = await _repository.GetCustomerByName("Linus Torvalds");

    Assert.IsNotNull(customer);

    var dbCustomer = _context.Customers.First();

    Assert.AreEqual(dbCustomer, customer);
  }

  [TestMethod]
  [DataRow("")]
  [DataRow(null)]
  [DataRow("#")]
  [DataRow("$")]
  [DataRow("%")]
  [DataRow("&")]
  [DataRow("*")]
  public async Task GetCustomerByName_Failure_InvailidName(string name)
  {
    var customer = await _repository.GetCustomerByName(name);

    Assert.IsNull(customer);
  }

  [TestMethod]
  public async Task GetCustomerByName_Failure_UserNameNotFound()
  {
    var customer = await _repository.GetCustomerByName("Bob Cat");

    Assert.IsNull(customer);
  }
}