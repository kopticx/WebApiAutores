using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validations;

public class PrimeraLetraMayusculaAttribute: ValidationAttribute
{
  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    if (string.IsNullOrEmpty(value.ToString()))
    {
      return ValidationResult.Success;
    }

    var primeraLetra = value.ToString().First().ToString();

    return primeraLetra != primeraLetra.ToUpper() ? new ValidationResult("La primera letra debe ser mayúscula") : ValidationResult.Success;
  }
}