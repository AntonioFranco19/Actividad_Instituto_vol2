using Microsoft.Extensions.DependencyInjection;
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

    public IReadOnlyList<Alumno> ObtenerAlumnos()
    {
        return _alumnos.Values.ToList();
    }

    public IReadOnlyList<Alumno> ObtenerAlumnosPorCicloFormativo(string cicloFormativoId)
    {
        return _alumnos.Values.Where(alumno => alumno.CicloFormativoId == cicloFormativoId).ToList();
    }

    public IReadOnlyList<Alumno> ObtenerAlumnosPorCentro(int centroId)
    {
        return _alumnos.Values.Where(alumno => alumno.CentroId == centroId).ToList();
    }

    public Alumno? ObtenerAlumnoPorNif(string nif)
    {
        return _alumnos.GetValueOrDefault(nif);
    }

    public Alumno InsertarAlumno(Alumno alumno)
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
        
        return _alumnos[alumno.Nif] = alumno;
    }

    public Alumno ModificarAlumno(Alumno alumno)
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
        
        return _alumnos[alumno.Nif] = alumno;
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