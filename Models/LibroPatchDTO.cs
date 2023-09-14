using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Models;

public class LibroPatchDTO
{
  [PrimeraLetraMayuscula]
  [StringLength(maximumLength: 250)]
  [Required]
  public string Titulo { get; set; }
  public DateTime FechaPublicacion { get; set; }
  public List<int> AutoresIds { get; set; }
}