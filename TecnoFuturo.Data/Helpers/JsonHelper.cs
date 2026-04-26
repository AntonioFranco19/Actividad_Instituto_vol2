using System.Text.Json;
using Microsoft.Extensions.Options;
using TecnoFuturo.Core.Options;

namespace TecnoFuturo.Data.Helpers;

public class JsonHelper
{
    private readonly string _directorioTrabajo;

    public JsonHelper(IOptions<DirectoryOption> option)
    {
        _directorioTrabajo = Path.Combine(Directory.GetCurrentDirectory(),  option.Value.Data);
            
            if (!Directory.Exists(_directorioTrabajo))
            {
                Directory.CreateDirectory(_directorioTrabajo);
            }
    }
    
    public List<T>? LeerDatos<T>(string archivo)
    {
        try
        {
            var ruta = Path.Combine(_directorioTrabajo, archivo);
            if (!File.Exists(ruta))
            {
                return null;
            }

            var json = File.ReadAllText(ruta);
            return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<List<T>>(json);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public void GuardarDatos<T>(string archivo, IEnumerable<T> datos)
    {
        try
        {
            var ruta = Path.Combine(_directorioTrabajo, archivo);
            var json = JsonSerializer.Serialize(datos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ruta, json);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
        }
    }
}