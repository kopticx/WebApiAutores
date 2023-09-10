namespace WebApiAutores.Models;

public class LibroDTOConAutores : LibroDTO
{
  public List<AutorDTO> Autores { get; set; }
}