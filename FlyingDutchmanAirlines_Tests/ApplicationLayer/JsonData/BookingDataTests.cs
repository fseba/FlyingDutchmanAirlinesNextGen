using System;
using System.ComponentModel.DataAnnotations;
using FlyingDutchmanAirlines.ApplicationLayer.JsonData;
using Microsoft.AspNetCore.Http;

namespace FlyingDutchmanAirlines_Tests.ApplicationLayer.JsonData;

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
  [DataRow("Mike", "")]
  [DataRow("", "Morand")]
  public void BookingData_SingleInvalidData(string firstName, string lastName)
  {
    BookingData bookingData = new()
    {
      FirstName = firstName,
      LastName = lastName
    };

    var validationResults = bookingData.Validate(new ValidationContext(bookingData));

    Assert.IsNotNull(validationResults);
    Assert.AreEqual("One name is null or whitespace", validationResults.First().ErrorMessage);
}

  [TestMethod]
  [DataRow("", "")]
  [DataRow(null, null)]
  public void BookingData_InvalidData_CompleteInvalidData(string firstName, string lastName)
  {
    BookingData bookingData = new()
    {
      FirstName = firstName,
      LastName = lastName
    };

    var validationResults = bookingData.Validate(new ValidationContext(bookingData));

    Assert.IsNotNull(validationResults);
    Assert.AreEqual("Both given names are null or whitespace", validationResults.First().ErrorMessage);
  }
}

