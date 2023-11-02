using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;
using WebApiAutores.Models;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/autores")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
public class AutoresController : ControllerBase
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<AutoresController> _logger;
  private readonly IMapper _mapper;

  public AutoresController(ApplicationDbContext context, ILogger<AutoresController> logger,
    IMapper mapper)
  {
    _context = context;
    _logger = logger;
    _mapper = mapper;
  }

  [HttpGet(Name = "obtenerAutores")]
  public async Task<IActionResult> Get()
  {
    _logger.LogInformation("Obteniendo los autores");

    var listaAutores = await _context.Autores
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

  private void GenerarEnlaces(AutorDTO autorDTO)
  {
    autorDTO.Enlaces.Add(new DatoHATEOAS(
      enlace: Url.Link("obtenerAutor", new { id = autorDTO.Id }),
      descripcion: "self",
      metodo: "GET"));

    autorDTO.Enlaces.Add(new DatoHATEOAS(
      enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id }),
      descripcion: "autor-actualizar",
      metodo: "PUT"));

    autorDTO.Enlaces.Add(new DatoHATEOAS(
      enlace: Url.Link("borrarAutor", new { id = autorDTO.Id }),
      descripcion: "autor-borrar",
      metodo: "DELETE"));
  }

  [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
  public async Task<IActionResult> Get([FromRoute] string nombre)
  {
    var autores = await _context.Autores
      .Where(x => x.Nombre.Contains(nombre))
      .ProjectTo<AutorDTO>(_mapper.ConfigurationProvider)
      .ToListAsync();

    return Ok(autores);
  }

  [HttpPost(Name = "crearAutor")]
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

  [HttpPut("{id:int}", Name = "actualizarAutor")]
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

  [HttpDelete("{id:int}", Name = "borrarAutor")]
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