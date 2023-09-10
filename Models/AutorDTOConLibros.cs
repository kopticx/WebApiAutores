namespace WebApiAutores.Models;

public class AutorDTOConLibros : AutorDTO
{
  public List<LibroDTO> Libros { get; set; }
}