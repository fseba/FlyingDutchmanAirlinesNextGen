using System.ComponentModel.DataAnnotations;

namespace FlyingDutchmanAirlines.ApplicationLayer.JsonData;

public class BookingData : IValidatableObject
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    List<ValidationResult> results = new();


    if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
    {
      results.Add(new ValidationResult("Both given names are null or whitespace"));
    }

    else if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
    {
      results.Add(new ValidationResult("One name is null or whitespace"));
    }

    return results;
  }
}

