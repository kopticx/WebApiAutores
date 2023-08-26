namespace WebApiAutores.Middlewares
{
  public static class LoguearRespuestaHTTPMiddlewareExtensions
  {
    public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
    {
      return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
    }
  }

  public class LoguearRespuestaHTTPMiddleware
  {
    private readonly RequestDelegate siguiente;
    private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

    public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LoguearRespuestaHTTPMiddleware> logger)
    {
      this.siguiente = siguiente;
      this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      using (var ms = new MemoryStream())
      {
        var cuerpoOriginalRespuesta = context.Response.Body;
        context.Response.Body = ms;

        await siguiente(context);

        ms.Seek(0, SeekOrigin.Begin);
        var respuesta = new StreamReader(ms).ReadToEnd();
        ms.Seek(0, SeekOrigin.Begin);

        await ms.CopyToAsync(cuerpoOriginalRespuesta);
        context.Response.Body = cuerpoOriginalRespuesta;

        logger.LogInformation(respuesta);
      }
    }
  }
}