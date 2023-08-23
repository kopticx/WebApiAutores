using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.entities;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase
{
  private readonly ApplicationDbContext _context;

  public AutoresController(ApplicationDbContext context)
  {
    _context = context;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    var listaAutores = await _context.Autores
            .Include(x => x.Libros)
            .ToListAsync();

    return Ok(listaAutores);
  }

  [HttpGet("primero")]
  public async Task<IActionResult> GetPrimero([FromHeader] int miValor, [FromQuery] string nombre)
  {
    var autor = await _context.Autores
            .Include(x => x.Libros)
            .FirstOrDefaultAsync();

    return Ok(autor);
  }

  [HttpGet("{id:int}")]
  //[HttpGet("{id:int}/{param2=pepe}")] => Parametro con valor por defecto
  //[HttpGet("{id:int}/{param2?}")] => Parametro opcional
  public async Task<IActionResult> Get(int id)
  {
    var autor = await _context.Autores
            .Include(x => x.Libros)
            .FirstOrDefaultAsync(x => x.Id == id);

    if (autor is null)
    {
      return NotFound();
    }

    return Ok(autor);
  }

  [HttpGet("{nombre}")]
  public async Task<IActionResult> Get([FromRoute] string nombre)
  {
    var autor = await _context.Autores
            .Include(x => x.Libros)
            .FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

    if (autor is null)
    {
      return NotFound();
    }

    return Ok(autor);
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] Autor autor)
  {
    var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

    if (existeAutorConElMismoNombre)
    {
      return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
    }

    _context.Add(autor);
    await _context.SaveChangesAsync();

    return Ok();
  }

  [HttpPut("{id:int}")]
  public async Task<IActionResult> Put(Autor autor, int id)
  {
    if (autor.Id != id)
    {
      return BadRequest("El id del autor no coincide con el id de la URL");
    }

    _context.Update(autor);
    await _context.SaveChangesAsync();

    return Ok();
  } 

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    var autor = await _context.Autores.FirstOrDefaultAsync(x => x.Id == id);

    if (autor is null)
    {
      return NotFound();
    }

    _context.Remove(autor);
    await _context.SaveChangesAsync();

    return Ok();
  }
}