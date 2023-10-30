using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;
using WebApiAutores.Models;
using WebApiAutores.Utilities;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/autores")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
public class AutoresController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly IMapper _mapper;

  public AutoresController(ApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<IActionResult> Get([FromQuery] PaginacionDTO paginacionDto)
  {
    var queryable = _context.Autores.AsQueryable();
    await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

    var listaAutores = await queryable
      .OrderBy(a => a.Nombre)
      .Paginar(paginacionDto)
      .ProjectTo<AutorDTO>(_mapper.ConfigurationProvider)
      .ToListAsync();

    return Ok(listaAutores);
  }

  [HttpGet("{id:int}", Name = "obtenerAutor")]
  public async Task<IActionResult> Get(int id)
  {
    var autor = await _context.Autores
      .Include(autorDb => autorDb.AutoresLibros)
      .ThenInclude(libroDb => libroDb.Libro)
      .FirstOrDefaultAsync(x => x.Id == id);

    if (autor is null)
    {
      return NotFound();
    }

    var autorDto = _mapper.Map<AutorDTOConLibros>(autor);

    return Ok(autorDto);
  }

  [HttpGet("{nombre}")]
  public async Task<IActionResult> Get([FromRoute] string nombre)
  {
    var autores = await _context.Autores
      .Where(x => x.Nombre.Contains(nombre))
      .ProjectTo<AutorDTO>(_mapper.ConfigurationProvider)
      .ToListAsync();

    return Ok(autores);
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] AutorCreacionDTO model)
  {
    var existeAutorConElMismoNombre = await _context.Autores.AnyAsync(x => x.Nombre == model.Nombre);

    if (existeAutorConElMismoNombre)
    {
      return BadRequest($"Ya existe un autor con el nombre {model.Nombre}");
    }

    var autor = _mapper.Map<Autor>(model);

    _context.Add(autor);
    await _context.SaveChangesAsync();

    var autorDto = _mapper.Map<AutorDTO>(autor);

    return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDto);
  }

  [HttpPut("{id:int}")]
  public async Task<IActionResult> Put(AutorCreacionDTO model, int id)
  {
    var existe = await _context.Autores.AnyAsync(x => x.Id == id);

    if (!existe)
    {
      return NotFound();
    }

    var autor = _mapper.Map<Autor>(model);
    autor.Id = id;

    _context.Update(autor);
    await _context.SaveChangesAsync();

    return NoContent();
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

    return NoContent();
  }
}