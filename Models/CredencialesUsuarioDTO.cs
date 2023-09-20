using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Models;

public class CredencialesUsuarioDTO
{
  [Required]
  public string UserName { get; set; }
  [Required]
  [EmailAddress]
  public string Email { get; set; }
  [Required]
  public string Password { get; set; }
}