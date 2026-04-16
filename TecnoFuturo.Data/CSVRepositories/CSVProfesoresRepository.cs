using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.Data.CSVRepositories;

public class CsvProfesoresRepository : IProfesorRepository
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, Profesor> _profesores;
    private const string SaveFile = "profesores.csv";
    
    public CsvProfesoresRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CargarDesdeArchivo();
    }

    private void GuardarEnArchivo()
    {
        try
        {
            using(StreamWriter writer = new StreamWriter(new FileStream(SaveFile, FileMode.OpenOrCreate)))
            {
                foreach (var value in _profesores.Values)
                {
                    writer.WriteLine($"{value.Nif};{value.Nombre};{value.Email};{value.Direccíon};{value.Telefono};{value.CentroId}");
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido guardar los datos");
            throw;
        }
    }

    private void CargarDesdeArchivo()
    {
        try
        {
            if (!File.Exists(SaveFile))
            {
                _profesores = new Dictionary<string, Profesor>();
                return;
            }

            using (StreamReader reader = new StreamReader(new FileStream(SaveFile, FileMode.Open)))
            {
                _profesores = new Dictionary<string, Profesor>();
                string? linea;
                while (string.IsNullOrWhiteSpace(linea = reader.ReadLine()))
                {
                    string[] datos = linea.Split(';');
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("No se han podido cargar los archivos");
            throw;
        }
    }
    
    private Alumno? ConvertirStringAProfesor(string? cadena)
    {
        if (cadena == null) return null;
        if (!Regex.IsMatch(cadena, @"^([^;]+);([^;]+);([^;]+);([^;]+);([^;]+);([^;]+)$")) return null;
        
        string[] campos = cadena.Split(';');

        if (campos.Length != 6)
        {
            return null;
        }

        if (!int.TryParse(campos[5], out int centroId))
        {
            centroId = 0;
        }
        return new Alumno
        {
            Nif = campos[0],
            Nombre = campos[1],
            Email = campos[2],
            Direccíon = campos[3],
            Telefono = campos[4],
            CentroId = centroId,
        };
    }
    
    public IReadOnlyList<ProfesorDTO> ObtenerProfesores()
    {
        return _profesores.Values.Select(x => ToMap(x)).ToList();
    }

    public IReadOnlyList<ProfesorDTO> ObtenerProfesoresPorCentro(int centroId)
    {
        return _profesores.Values.Where(p => p.CentroId == centroId).Select(x=> ToMap(x)).ToList();
    }

    public ProfesorDTO? ObtenerProfesorPorNif(string nif)
    {
        Profesor? profesor = _profesores.GetValueOrDefault(nif);
        if (profesor != null) return ToMap(profesor);
        return null;
    }

    public ProfesorDTO InsertarProfesor(Profesor profesor)
    {
        var centroRepository = _serviceProvider.GetRequiredService<ICentroRepository>();
        if (centroRepository.ObtenerCentroPorId(profesor.CentroId) == null)
        {
            throw new ArgumentException("El centro especificado no existe", nameof(profesor));
        }

        if (_profesores.ContainsKey(profesor.Nif))
        {
            throw new ArgumentException("El profesor ya existe", nameof(profesor));
        }
        
        _profesores[profesor.Nif] = profesor;
        GuardarEnArchivo();
        return ToMap(profesor);
    }

    public ProfesorDTO ModificarProfesor(Profesor profesor)
    {
        var centroRepository = _serviceProvider.GetRequiredService<ICentroRepository>();
        if (centroRepository.ObtenerCentroPorId(profesor.CentroId) == null)
        {
            throw new ArgumentException("El centro especificado no existe", nameof(profesor));
        }

        if (!_profesores.ContainsKey(profesor.Nif))
        {
            throw new ArgumentException("El profesor no existe", nameof(profesor));
        }
        
        _profesores[profesor.Nif] = profesor;
        GuardarEnArchivo();
        return ToMap(profesor);
    }

    public bool BorrarProfesor(string nif)
    {
        var profesor = _profesores.GetValueOrDefault(nif);

        if (profesor == null)
        {
            throw new ArgumentException("El profesor no existe", nameof(nif));
        }

        var moduloRepository = _serviceProvider.GetRequiredService<IModuloRepository>();
        var modulosPorProfesor = moduloRepository.ObtenerModulosPorProfesor(nif);
        if (modulosPorProfesor.Count != 0) throw new InvalidOperationException("El profesor tiene modulos asignados");

        bool exito = _profesores.Remove(nif);
        if (exito) GuardarEnArchivo();
        return exito;
    }

    private ProfesorDTO ToMap(Profesor a)
    {
        return new ProfesorDTO(
            Nif: a.Nif, 
            Nombre: a.Nombre, 
            Email: a.Email, 
            Direccion: a.Direccíon, 
            Telefono: a.Telefono,
            CentroId: a.CentroId
            );
    }
}