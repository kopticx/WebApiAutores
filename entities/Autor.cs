using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entities;

public class Autor
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(maximumLength: 30, ErrorMessage = "El campo {0} no debe estar entre {2} y {1} caracteres", MinimumLength = 5)]
    [PrimeraLetraMayuscula]
    public string Nombre { get; set; }
}