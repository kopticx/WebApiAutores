using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
      .Where(x => model.AutoresIds.Contains(x.Id))
      .Select(x => x.Id)
      .ToListAsync();

    if (model.AutoresIds.Count != autoresIds.Count)
    {
      return BadRequest("No existe uno o más autores enviados");
    }

    var libro = _mapper.Map<Libro>(model);

    AsignarOrdenAutores(libro);

    _context.Add(libro);
    await _context.SaveChangesAsync();

    var libroDto = _mapper.Map<LibroDTO>(libro);

    return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDto);
  }

  [HttpPut("{id:int}")]
  public async Task<IActionResult> Put(int id, LibroCreacionDTO model)
  {
    var libroDb = await _context.Libros
      .Include(x => x.AutoresLibros)
      .FirstOrDefaultAsync(x => x.Id == id);

    if (libroDb is null)
    {
      return NotFound();
    }

    libroDb = _mapper.Map(model, libroDb);

    AsignarOrdenAutores(libroDb);

    await _context.SaveChangesAsync();

    return NoContent();
  }

  [HttpPatch("{id:int}")]
  public async Task<IActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
  {
    if(patchDocument is null)
    {
      return BadRequest();
    }

    var libroDB = await _context.Libros.FirstOrDefaultAsync(x => x.Id == id);

    if (libroDB is null)
    {
      return NotFound();
    }

    var libroDTO = _mapper.Map<LibroPatchDTO>(libroDB);

    patchDocument.ApplyTo(libroDTO, ModelState);

    var esValido = TryValidateModel(libroDTO);

    if (!esValido)
    {
      return BadRequest(ModelState);
    }

    _mapper.Map(libroDTO, libroDB);

    await _context.SaveChangesAsync();

    return NoContent();
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);

    if (libro is null)
    {
      return NotFound();
    }

    _context.Remove(libro);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  private void AsignarOrdenAutores(Libro libro)
  {
    if (libro.AutoresLibros is not null)
    {
      for (int i = 0; i < libro.AutoresLibros.Count; i++)
      {
        libro.AutoresLibros[i].Orden = i;
      }
    }
  }
}