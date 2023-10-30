using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Utilities;

public static class HttpContextExtensions
{
  public static async Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext, IQueryable<T> queryable)
  {
    if (httpContext is null)
    {
      throw new ArgumentNullException(nameof(httpContext));
    }

    double cantidad = await queryable.CountAsync();
    httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());
  }
}