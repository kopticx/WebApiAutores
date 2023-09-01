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

  [HttpGet("{id:int}")]
  public async Task<IActionResult> Get(int id)
  {
    var libro = await _context.Libros
          .Include(c => c.Comentarios)
          .FirstOrDefaultAsync(x => x.Id == id);

    if (libro is null)
    {
      return NotFound();
    }

    var libroDTO = _mapper.Map<LibroDTO>(libro);

    return Ok(libroDTO);
  }

  [HttpPost]
  public async Task<IActionResult> Post(LibroCreacionDTO model)
  {
    // var existeAutor = await _context.Autores.AnyAsync(x => x.Id == libroDTO);
    //
    // if (!existeAutor)
    // {
    //   return BadRequest($"No existe el autor con el Id {libroDTO.AutorId}");
    // }

    var libro = _mapper.Map<Libro>(model);

    _context.Add(libro);
    await _context.SaveChangesAsync();

    return Ok("Libro creado exitosamente");
  }
}