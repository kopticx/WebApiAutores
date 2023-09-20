using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;

namespace WebApiAutores;

public class ApplicationDbContext : IdentityDbContext
{
  public ApplicationDbContext(DbContextOptions options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    //Usando el Api fluyenye para PK compuestas

    // modelBuilder.Entity<AutorLibro>()
    // .HasKey(al => new {al.AutorId, al.LibroId});
  }

  public DbSet<Autor> Autores { get; set; }
  public DbSet<Libro> Libros { get; set; }
  public DbSet<Comentario> Comentarios { get; set; }
  public DbSet<AutorLibro> AutoresLibros { get; set; }
}