using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validations;

public class PrimeraLetraMayusculaAttribute: ValidationAttribute
{
  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    if (String.IsNullOrEmpty(value.ToString()))
    {
      return ValidationResult.Success;
    }

    var primeraLetra = value.ToString().First().ToString();

    if(primeraLetra != primeraLetra.ToUpper())
    {
      return new ValidationResult("La primera letra debe ser mayúscula");
    }

    return ValidationResult.Success;
  }
}