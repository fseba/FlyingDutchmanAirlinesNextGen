using System.ComponentModel.DataAnnotations;

namespace FlyingDutchmanAirlines.ControllerLayer.JsonData;

public class BookingData : IValidatableObject
{
  private string _firstName = null!;
  private string _lastName = null!;

  public string? FirstName
  {
    get => _firstName;
    set => _firstName = ValidateName(value!, nameof(FirstName));
  }

  public string? LastName
  {
    get => _lastName;
    set => _lastName = ValidateName(value!, nameof(LastName));
  }

  private string ValidateName(string name, string propertyName) =>  
    string.IsNullOrWhiteSpace(name)
      ? throw new BadHttpRequestException($"could not set {propertyName} - no name provided")
      : name;

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    List<ValidationResult> results = new();

    if (FirstName == null && LastName == null)
    {
      results.Add(new ValidationResult("All given data points are null"));
    }
    else if (FirstName == null || LastName == null)
    {
      results.Add(new ValidationResult("One of the given data points is null"));
    }

    return results;
  }
}

