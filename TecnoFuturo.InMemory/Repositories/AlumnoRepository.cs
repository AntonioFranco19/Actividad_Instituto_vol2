using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.InMemory.Repositories;

public class AlumnoRepository : IAlumnoRepository
{
    
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Alumno> _alumnos = [];

    public AlumnoRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        
        return ToMap(_alumnos[alumno.Nif] = alumno);
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
        
        return ToMap(_alumnos[alumno.Nif] = alumno);
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

    public bool BorrarAlumno(string nif)
    {
        if (!_alumnos.ContainsKey(nif))
        {
            return false;
        }
        return _alumnos.Remove(nif);
    }
}