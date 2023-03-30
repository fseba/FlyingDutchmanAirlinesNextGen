using FlyingDutchmanAirlines.DatabaseLayer.Models;

namespace FlyingDutchmanAirlines_Tests.Models;

[TestClass]
public class CustomerModelTests
{
  [TestMethod]
  public void CustomerCreate_Success()
  {
    var result = Customer.Create("Bob");

    Assert.AreEqual("Bob", result!.Name);
  }


  [TestMethod]
  [DataRow(null)]
  [DataRow("")]
  public void CustomerCreate_Failure_NameIsNullOrEmptyString(string name)
  {
    var result = Customer.Create(name);
    Assert.IsNull(result);
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
  public void CustomerCreate_Failure_NameContainsInvalidCharacters(char invalidCharacter)
  {
    var result = Customer.Create($"Donald Knuth{invalidCharacter}");
    Assert.IsNull(result);
  }
}

