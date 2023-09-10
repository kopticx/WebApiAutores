using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;
using WebApiAutores.Models;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/libros/{libroId:int}/comentarios")]
public class ComentariosController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly IMapper _mapper;

  public ComentariosController(ApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<IActionResult> Get(int libroId)
  {
    var existeLibro = await _context.Libros.AnyAsync(l => l.Id == libroId);

    if (!existeLibro)
    {
      return NotFound();
    }

    var comentarios = await _context.Comentarios
          .Where(c => c.LibroId == libroId)
          .ProjectTo<ComentarioDTO>(_mapper.ConfigurationProvider)
          .ToListAsync();

    return Ok(comentarios);
  }

  [HttpGet("{id:int}", Name = "obtenerComentario")]
  public async Task<IActionResult> GetById(int libroId, int id)
  {
    var existeLibro = await _context.Libros.AnyAsync(l => l.Id == libroId);

    if (!existeLibro)
    {
      return NotFound();
    }

    var comentario = await _context.Comentarios
          .FirstOrDefaultAsync(c => c.Id == id);

    if (comentario is null)
    {
      return NotFound();
    }

    var comentarioDto = _mapper.Map<ComentarioDTO>(comentario);

    return Ok(comentarioDto);
  }

  [HttpPost]
  public async Task<IActionResult> Post(int libroId, ComentarioCreacionDTO model)
  {
    var existeLibro = await _context.Libros.AnyAsync(l => l.Id == libroId);

    if (!existeLibro)
    {
      return NotFound();
    }

    var comentario = _mapper.Map<Comentario>(model);
    comentario.LibroId = libroId;

    _context.Add(comentario);
    await _context.SaveChangesAsync();

    var comentarioDto = _mapper.Map<ComentarioDTO>(comentario);

    return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId = comentario.LibroId }, comentarioDto);
  }
}