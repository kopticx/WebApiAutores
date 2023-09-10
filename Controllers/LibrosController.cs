using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;
using WebApiAutores.Models;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly IMapper _mapper;

  public LibrosController(ApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpGet("{id:int}", Name = "obtenerLibro")]
  public async Task<IActionResult> Get(int id)
  {
    var libro = await _context.Libros
          .Include(libro => libro.AutoresLibros)
          .ThenInclude(autorLibro => autorLibro.Autor)
          .Include(c => c.Comentarios)
          .FirstOrDefaultAsync(x => x.Id == id);

    if (libro is null)
    {
      return NotFound();
    }

    libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

    var libroDto = _mapper.Map<LibroDTOConAutores>(libro);

    return Ok(libroDto);
  }

  [HttpPost]
  public async Task<IActionResult> Post(LibroCreacionDTO model)
  {
    if (model.AutoresIds is null)
    {
      return BadRequest("No se puede crear un libro sin autores");
    }

    var autoresIds = await _context.Autores
          .Where(x =>  model.AutoresIds.Contains(x.Id))
          .Select(x => x.Id)
          .ToListAsync();

    if (model.AutoresIds.Count() != autoresIds.Count())
    {
      return BadRequest("No existe uno o más autores enviados");
    }

    var libro = _mapper.Map<Libro>(model);

    if(libro.AutoresLibros is not null)
    {
      for (int i = 0; i < libro.AutoresLibros.Count; i++)
      {
        libro.AutoresLibros[i].Orden = i;
      }
    }

    _context.Add(libro);
    await _context.SaveChangesAsync();

    var libroDto = _mapper.Map<LibroDTO>(libro);

    return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDto);
  }
}