using AutoMapper;
using WebApiAutores.Entities;
using WebApiAutores.Models;

namespace WebApiAutores.Utilities;

public class AutoMapperProfiles : Profile
{
  public AutoMapperProfiles()
  {
    CreateMap<AutorCreacionDTO, Autor>();
    CreateMap<Autor, AutorDTO>();

    CreateMap<Autor, AutorDTOConLibros>()
          .ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));

    CreateMap<LibroCreacionDTO, Libro>()
          .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapearLibrosAutores));

    CreateMap<Libro, LibroDTO>();

    CreateMap<Libro, LibroDTOConAutores>()
          .ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));

    CreateMap<ComentarioCreacionDTO, Comentario>();
    CreateMap<Comentario, ComentarioDTO>();
  }

  private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
  {
    var resultado = new List<LibroDTO>();

    if (autor.AutoresLibros is null)
    {
      return resultado;
    }

    foreach (var autorLibro in autor.AutoresLibros)
    {
      resultado.Add(new LibroDTO() { Id = autorLibro.LibroId, Titulo = autorLibro.Libro.Titulo });
    }

    return resultado;
  }

  private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
  {
    var resultado = new List<AutorDTO>();

    if (libro.AutoresLibros is null)
    {
      return resultado;
    }

    foreach (var autorLibro in libro.AutoresLibros)
    {
      resultado.Add(new AutorDTO() { Id = autorLibro.AutorId, Nombre = autorLibro.Autor.Nombre });
    }

    return resultado;
  }

  private List<AutorLibro> MapearLibrosAutores(LibroCreacionDTO libroCreacionDTO, Libro libro)
  {
    var resultado = new List<AutorLibro>();

    if (libroCreacionDTO.AutoresIds is null)
    {
      return resultado;
    }

    foreach (var autoresId in libroCreacionDTO.AutoresIds)
    {
      resultado.Add(new AutorLibro() { AutorId = autoresId });
    }

    return resultado;
  }
}