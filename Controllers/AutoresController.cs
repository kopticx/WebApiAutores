using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<AutoresController> _logger;

  public AutoresController(ApplicationDbContext context, ILogger<AutoresController> logger)
  {
    _context = context;
    _logger = logger;
  }

  [HttpGet]
  //[ResponseCache(Duration = 20)]
  [ServiceFilter(typeof(MiFiltroDeAccion))]
  public async Task<IActionResult> Get()
  {
    _logger.LogInformation("Obteniendo los autores");

    var listaAutores = await _context.Autores
            .ToListAsync();

    return Ok(listaAutores);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> Get(int id)
  {
    var autor = await _context.Autores
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