using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Entities;

[PrimaryKey(nameof(LibroId), nameof(AutorId))]
public class AutorLibro
{
  public int LibroId { get; set; }
  public Libro Libro { get; set; }

  public int AutorId { get; set; }
  public Autor Autor { get; set; }

  public int Orden { get; set; }
}