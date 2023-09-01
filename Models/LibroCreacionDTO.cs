using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Models;

public class LibroCreacionDTO
{
  [PrimeraLetraMayuscula]
  [StringLength(maximumLength: 250)]
  public string Titulo { get; set; }
  public List<int> AutoresIds { get; set; }
}