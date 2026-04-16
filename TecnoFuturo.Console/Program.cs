using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TecnoFuturo.App.Configuraciones;
using TecnoFuturo.App.Servicios;
using TecnoFuturo.Console.Servicios;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Core.Servicios;
using TecnoFuturo.Data.Repsoitories;
using TecnoFuturo.InMemory.Repositories;

namespace TecnoFuturo.Console;

class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // 1. Cargar explícitamente el appsettings.json (si no lo hace por defecto)
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // 2. VINCULACIÓN (Binding):
        // Decimos: "Toma la sección 'ConfiguracionCentro' del JSON y rellena la clase ConfiguracionCentro"
        builder.Services.Configure<ConfiguracionCentro>(builder.Configuration.GetSection(nameof(ConfiguracionCentro)));

        // 3. Registramos nuestro servicio consumidor
        builder.Services.AddSingleton<IMensageServicio, MensajeServicio>();
        builder.Services.AddSingleton<ICentroRepository, CentroRepository>();
        builder.Services.AddSingleton<IModuloRepository, ModuloRepository>();
        builder.Services.AddSingleton<IProfesorRepository, JsonProfesorRepository>();
        builder.Services.AddSingleton<IAlumnoRepository, JsonAlumnoRepository>();
        builder.Services.AddSingleton<ICicloFormativoRepository, JsonCicloFormativoRepository>();
        // builder.Services.AddSingleton<IAlumnoRepository, JsonAlumnoRepository>();
        builder.Services.AddSingleton<CentroServicio>();
        var host = builder.Build();

        // 4. Ejecución
        var servicio = host.Services.GetRequiredService<CentroServicio>();
        servicio.Run();
    }
}