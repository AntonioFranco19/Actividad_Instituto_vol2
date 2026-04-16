using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Core.Validators;

namespace TecnoFuturo.Data.CSVRepositories;

public class CsvAlumnoRepository : IAlumnoRepository
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, Alumno> _alumnos;
    private const string SaveFile = "alumnos.csv";

    public CsvAlumnoRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CargarDatos();
    }

    public IReadOnlyList<AlumnoDTO> ObtenerAlumnos()
    {
        return _alumnos.Values.Select(x => ToMap(x)).ToList();
    }

    private void GuardarEnArchivo()
    {
        try
        {
            using FileStream fileStream = new FileStream(SaveFile, FileMode.OpenOrCreate);
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                foreach (var value in _alumnos.Values)
                {
                    writer.WriteLine($"{value.Nif};{value.Nombre};{value.Email};{value.Direccíon};{value.Telefono};{value.CentroId};{value.CicloFormativoId}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("No se ha podido guardar");
            throw;
        }
    }

    private void CargarDatos()
    {
        try
        {
            if (!File.Exists(SaveFile))
            {
                _alumnos = new Dictionary<string, Alumno>();
                return;
            }
            
            using (FileStream fs = new FileStream(SaveFile, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    _alumnos = new Dictionary<string, Alumno>();
                    string? linea;
                    while ((linea = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(linea)) continue;
                        
                        Alumno? al = ConvertirStringAAlumno(linea);
                        if (al != null)
                        {
                            _alumnos.TryAdd(al.Nif, al);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("No se han podido cargar los datos;");
            throw;
        }
    }

    private Alumno? ConvertirStringAAlumno(string? cadena)
    {
        if (cadena == null) return null;
        if (!Regex.IsMatch(cadena, @"^([^;]+);([^;]+);([^;]+);([^;]+);([^;]+);([^;]+);([^;]+)$")) return null;
        
        string[] campos = cadena.Split(';');

        if (campos.Length != 7)
        {
            return null;
        }

        if (!int.TryParse(campos[5], out int centroId))
        {
            centroId = 0;
        }
        Alumno al = new Alumno
        {
            Nif = campos[0],
            Nombre = campos[1],
            Email = campos[2],
            Direccíon = campos[3],
            Telefono = campos[4],
            CentroId = centroId,
            CicloFormativoId = campos[6]
        };
        
        AlumnoValidator validator = new AlumnoValidator();

        return validator.Validate(al) ? al : null;
    }

    public IReadOnlyList<AlumnoDTO> ObtenerAlumnosPorCicloFormativo(string cicloFormativoId)
    {
        return _alumnos.Values.Where(alumno => alumno.CicloFormativoId == cicloFormativoId).Select(x => ToMap(x)).ToList();
    }

    public IReadOnlyList<AlumnoDTO> ObtenerAlumnosPorCentro(int centroId)
    {
        return _alumnos.Values.Where(alumno => alumno.CentroId == centroId).Select(x => ToMap(x)).ToList();
    }

    public AlumnoDTO? ObtenerAlumnoPorNif(string nif)
    {
        Alumno? alumno = _alumnos.GetValueOrDefault(nif);
        if (alumno != null) return ToMap(alumno);
        return null;
    }

    public AlumnoDTO InsertarAlumno(Alumno alumno)
    {
        if (_alumnos.ContainsKey(alumno.Nif))
        {
            throw new ArgumentException("El alumno ya existe", nameof(alumno));
        }

        var centroRepository = _serviceProvider.GetRequiredService<ICentroRepository>();
        if (centroRepository.ObtenerCentroPorId(alumno.CentroId) == null)
        {
            throw new ArgumentException("El centro especificado no existe", nameof(alumno));
        }
        
        _alumnos[alumno.Nif] = alumno;
        GuardarEnArchivo();
        return ToMap(alumno);
    }

    public AlumnoDTO ModificarAlumno(Alumno alumno)
    {
        if (!_alumnos.ContainsKey(alumno.Nif))
        {
            throw new ArgumentException("El alumno no existe", nameof(alumno));
        }
        
        var centroRepository = _serviceProvider.GetRequiredService<ICentroRepository>();
        if (centroRepository.ObtenerCentroPorId(alumno.CentroId) == null)
        {
            throw new ArgumentException("El centro especificado no existe", nameof(alumno));
        }
        
        _alumnos[alumno.Nif] = alumno;
        GuardarEnArchivo();
        return ToMap(alumno);
    }
    
    public bool BorrarAlumno(string nif)
    {
        if (!_alumnos.ContainsKey(nif))
        {
            return false;
        }
        bool exito = _alumnos.Remove(nif);
        if (exito) GuardarEnArchivo();
        return exito;
    }
    
    private AlumnoDTO ToMap(Alumno a)
    {
        return new AlumnoDTO(
            Nif: a.Nif, 
            Nombre: a.Nombre, 
            Email: a.Email, 
            Direccion: a.Direccíon, 
            Telefono: a.Telefono,
            CentroId: a.CentroId, 
            CicloFormativoId: a.CicloFormativoId);
    }
}