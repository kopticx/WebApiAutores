using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
  private readonly UserManager<IdentityUser> _userManager;

  public ComentariosController(ApplicationDbContext context, IMapper mapper,
    UserManager<IdentityUser> userManager)
  {
    _context = context;
    _mapper = mapper;
    _userManager = userManager;
  }

  [HttpGet(Name = "obtenerComentariosLibro")]
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

  [HttpPost(Name = "crearComentario")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> Post(int libroId, ComentarioCreacionDTO model)
  {
    var emailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
    var email = emailClaim.Value;

    var usuario = await _userManager.FindByEmailAsync(email);

    var existeLibro = await _context.Libros.AnyAsync(l => l.Id == libroId);

    if (!existeLibro)
    {
      return NotFound();
    }

    var comentario = _mapper.Map<Comentario>(model);
    comentario.LibroId = libroId;
    comentario.UsuarioId = usuario.Id;

    _context.Add(comentario);
    await _context.SaveChangesAsync();

    var comentarioDto = _mapper.Map<ComentarioDTO>(comentario);

    return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId = comentario.LibroId }, comentarioDto);
  }

  [HttpPut("{id:int}", Name = "actualizarComentario")]
  public async Task<IActionResult> Put(int libroId, int id, ComentarioCreacionDTO model)
  {
    var existeLibro = await _context.Libros.AnyAsync(l => l.Id == libroId);

    if (!existeLibro)
    {
      return NotFound();
    }

    var existeComentario = await _context.Comentarios.AnyAsync(c => c.Id == id);

    if (!existeComentario)
    {
      return NotFound();
    }

    var comentario = _mapper.Map<Comentario>(model);
    comentario.Id = id;
    comentario.LibroId = libroId;

    _context.Update(comentario);
    await _context.SaveChangesAsync();

    return NoContent();
  }
}