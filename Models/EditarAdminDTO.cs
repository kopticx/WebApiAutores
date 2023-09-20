using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Models;

public class EditarAdminDTO
{
  [Required]
  [EmailAddress]
  public string Email { get; set; }
}