using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Models;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api")]
public class RootController : ControllerBase
{
  [HttpGet(Name = "ObtenerRoot")]
  public IActionResult ObtenerRoot()
  {
    var datosHateoas = new List<DatoHATEOAS> { new (enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "GET") };

    return Ok(datosHateoas);
  }
}