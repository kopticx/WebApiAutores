using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filters;

public class MiFiltroDeAccion: IActionFilter
{
  private readonly ILogger<MiFiltroDeAccion> _logger;

  public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> logger)
  {
    _logger = logger;
  }

  public void OnActionExecuting(ActionExecutingContext context)
  {
    _logger.LogInformation("Antes de ejecutar la accion");
  }

  public void OnActionExecuted(ActionExecutedContext context)
  {
    _logger.LogInformation("Despues de ejecutar la accion");
  }
}