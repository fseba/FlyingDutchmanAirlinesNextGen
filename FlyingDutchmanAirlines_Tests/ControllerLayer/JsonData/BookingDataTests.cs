using System;
using FlyingDutchmanAirlines.ControllerLayer.JsonData;

namespace FlyingDutchmanAirlines_Tests.ControllerLayer.JsonData;

[TestClass]
public class BookingDataTests
{
  [TestMethod]
  public void BookingData_ValidData()
  {
    BookingData bookingData = new()
    {
      FirstName = "Bob",
      LastName = "Test"
    };

    Assert.AreEqual("Bob", bookingData.FirstName);
    Assert.AreEqual("Test", bookingData.LastName);
  }

  [TestMethod]
  [DataRow("Mike", null)]
  [DataRow(null, "Morand")]
  [ExpectedException(typeof(InvalidOperationException))]
  public void BookingData_InvalidData_NullPointers(string firstName, string lastName)
  {
    BookingData bookingData = new()
    {
      FirstName = firstName,
      LastName = lastName
    };
  }

  [TestMethod]
  [DataRow("Mike", "")]
  [DataRow("", "Morand")]
  [ExpectedException(typeof(InvalidOperationException))]
  public void BookingData_InvalidData_EmptyString(string firstName, string lastName)
  {
    BookingData bookingData = new()
    {
      FirstName = firstName,
      LastName = lastName
    };
  }
}

