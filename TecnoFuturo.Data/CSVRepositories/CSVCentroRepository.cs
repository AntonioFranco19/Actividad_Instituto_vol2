using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.Data.CSVRepositories;

public class CsvCentroRepository : ICentroRepository
{
    private Dictionary<int, Centro> _centros;
    private readonly IServiceProvider _serviceProvider;
    private const string SaveFile = "centros.csv";

    public CsvCentroRepository(IServiceProvider serviceProvider)
    {
       _serviceProvider = serviceProvider;
       CargarDatos();
    }
    
    private void GuardarEnArchivo()
    {
        try
        {
            using (FileStream fs = new FileStream(SaveFile, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (var value in _centros.Values)
                    {
                        writer.WriteLine($"{value.CentroId};{value.Nombre};{value.Direccion};{value.Telefono}");
                    }
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido guardar los datos");
            throw;
        }
    }

    private void CargarDatos()
    {
        try
        {
            if (!File.Exists(SaveFile))
            {
                _centros = new Dictionary<int, Centro>();
                return;
            }

            using (StreamReader reader = new StreamReader(new FileStream(SaveFile, FileMode.Open)))
            {
                _centros = new Dictionary<int, Centro>();
                string? linea;
                while (string.IsNullOrWhiteSpace(linea = reader.ReadLine()))
                {
                    Centro? centro = ConvertirStringACentro(linea);
                    if (centro != null)
                    {
                        _centros.TryAdd(centro.CentroId, centro);
                    }
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido cargar los datos");
            throw;
        }
    }
    
    private Centro? ConvertirStringACentro(string? cadena)
    {
        if (cadena == null) return null;
        if (!Regex.IsMatch(cadena, @"^([^;]+);([^;]+);([^;]+);([^;]+)$")) return null;
        
        string[] campos = cadena.Split(';');

        if (campos.Length != 4)
        {
            return null;
        }

        if (!int.TryParse(campos[0], out int centroId))
        {
            centroId = 0;
        }
        return new Centro
        {
            CentroId = centroId,
            Nombre = campos[1],
            Direccion = campos[2],
            Telefono = campos[3],
        };
    }
    
    public IReadOnlyList<CentroDTO> ObtenerCentros()
    {
        return _centros.Values.Select(x => ToMap(x)).ToList();
    }

    public CentroDTO? ObtenerCentroPorId(int id)
    {
        Centro? centro = _centros.GetValueOrDefault(id);
        if (centro != null) return ToMap(centro);
        return null;
    }

    public CentroDTO InsertarCentro(Centro centro)
    {
        bool exito = _centros.TryAdd(centro.CentroId, centro);
        if (exito)
        {
            GuardarEnArchivo();
            return ToMap(centro);
        }
        throw new InvalidOperationException("El centro ya existe");
    }

    public CentroDTO ModificarCentro(Centro centro)
    {
        if (!_centros.ContainsKey(centro.CentroId))
        {
            throw new InvalidOperationException("El centro no existe");
        }

        _centros[centro.CentroId] = centro;
        GuardarEnArchivo();
        return ToMap(centro);
    }

    public bool BorrarCentro(int id)
    {
        if (!_centros.TryGetValue(id, out var centro)) return false;
        
        var cicloFormativoRepository = _serviceProvider.GetRequiredService<ICicloFormativoRepository>();
        var ciclosFormativos = cicloFormativoRepository.ObtenerCiclosFormativosPorCentro(id);
        if (ciclosFormativos.Count != 0)
        {
            throw new InvalidOperationException("El centro tiene ciclos formativos asociados");
        }
        
        var alumnoRepository = _serviceProvider.GetRequiredService<IAlumnoRepository>();
        var alumnos = alumnoRepository.ObtenerAlumnosPorCentro(id);
        if (alumnos.Count != 0)
        {
            throw new InvalidOperationException("El centro tiene alumnos asociados");
        }

        var profesorRepository = _serviceProvider.GetRequiredService<IProfesorRepository>();
        var profesores = profesorRepository.ObtenerProfesoresPorCentro(id);
        if (profesores.Count != 0)
        {
            throw new InvalidOperationException("El centro tiene profesores asociados");
        }
        
        bool exito = _centros.Remove(id);
        if (exito) GuardarEnArchivo();
        return exito;
    }
    
    private CentroDTO ToMap(Centro a)
    {
        return new CentroDTO(
            CentroId: a.CentroId, 
            Nombre: a.Nombre, 
            Direccion: a.Direccion, 
            Telefono: a.Telefono
        );
    }
}