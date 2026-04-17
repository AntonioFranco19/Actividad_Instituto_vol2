using System.Text.Json;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.Data.BINDirectories;

public class BinAlumnoRepository : IAlumnoRepository
{
     private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, Alumno> _alumnos;
    private const string SaveFile = "alumnos.bin";

    public BinAlumnoRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CargarDesdeArchivo();
    }

    private void GuardarEnArchivo()
    {
        try
        {
            byte[] datos = MessagePackSerializer.Serialize(_alumnos.Values);
            File.WriteAllBytes(SaveFile, datos);
        }
        catch (Exception)
        {
            throw new InvalidOperationException("No se ha podido cargar en la memoria");
        }
    }

    private void CargarDesdeArchivo()
    {
        try
        {
            if (!File.Exists(SaveFile))
            {
                _alumnos = new Dictionary<string, Alumno>();
                return;
            }

            byte[] datos = File.ReadAllBytes(SaveFile);
            var lista = MessagePackSerializer.Deserialize<List<Alumno>>(datos);
            _alumnos = lista.ToDictionary(x => x.Nif);

        }
        catch (Exception e)
        {
            throw new InvalidCastException("No se ha podido cargar");
        }
    }

    public IReadOnlyList<AlumnoDTO> ObtenerAlumnos()
    {
        return _alumnos.Values.Select(x => ToMap(x)).ToList();
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
        if(exito) GuardarEnArchivo();
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