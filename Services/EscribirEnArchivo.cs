namespace WebApiAutores.Services;

public class EscribirEnArchivo : IHostedService
{
  private readonly IWebHostEnvironment _env;
  private readonly string _nombreArchivo = "Archivo 1.txt";
  private Timer _timer;

  public EscribirEnArchivo(IWebHostEnvironment env)
  {
    _env = env;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    Escribir("Proceso Iniciado");
    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _timer.Dispose();
    Escribir("Proceso Finalizado");
    return Task.CompletedTask;
  }

  private void DoWork(object state)
  {
    Escribir($"Proceso en ejecución: {DateTime.Now:dd/MM/yyyy hh:mm:ss}");
  }

  private void Escribir(string mensaje)
  {
    var ruta = $@"{_env.ContentRootPath}\wwwroot\{_nombreArchivo}";

    using (StreamWriter sw = new StreamWriter(ruta, append: true))
    {
      sw.WriteLine(mensaje);
    }
  }
}