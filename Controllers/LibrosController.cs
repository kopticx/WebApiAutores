using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
  private readonly ApplicationDbContext _context;

  public LibrosController(ApplicationDbContext context)
  {
    _context = context;
  }

  // [HttpGet("{id:int}")]
  // public async Task<IActionResult> Get(int id)
  // {
  //   var libro = await _context.Libros
  //           .FirstOrDefaultAsync(x => x.Id == id);
  //
  //   if (libro is null)
  //   {
  //     return NotFound();
  //   }
  //
  //   return Ok(libro);
  // }
  //
  // [HttpPost]
  // public async Task<IActionResult> Post(Libro libroDTO)
  // {
  //   var existeAutor = await _context.Autores.AnyAsync(x => x.Id == libroDTO);
  //
  //   if (!existeAutor)
  //   {
  //     return BadRequest($"No existe el autor con el Id {libroDTO.AutorId}");
  //   }
  //
  //   _context.Add(libroDTO);
  //   await _context.SaveChangesAsync();
  //
  //   return Ok("Libro creado exitosamente");
  // }
}