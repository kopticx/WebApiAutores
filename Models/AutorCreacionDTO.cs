using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Models;

public class AutorCreacionDTO
{
  [Required(ErrorMessage = "El campo {0} es requerido")]
  [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe estar entre {2} y {1} caracteres", MinimumLength = 5)]
  [PrimeraLetraMayuscula]
  public string Nombre { get; set; }
}