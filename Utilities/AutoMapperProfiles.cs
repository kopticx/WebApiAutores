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

    CreateMap<LibroCreacionDTO, Libro>();
    CreateMap<Libro, LibroDTO>();

    CreateMap<ComentarioCreacionDTO, Comentario>();
    CreateMap<Comentario, ComentarioDTO>();
  }
}