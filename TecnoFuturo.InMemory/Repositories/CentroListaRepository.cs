using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.InMemory.Repositories;

public class CentroListaRepository : ICentroRepository
{
    private readonly List<Centro> _centros = [];
    private readonly IServiceProvider _serviceProvider;

    public CentroListaRepository(IServiceProvider serviceProvider)
    {
       _serviceProvider = serviceProvider;
    }
    
    public IReadOnlyList<Centro> ObtenerCentros()
    {
        return _centros.ToList();
    }

    public Centro? ObtenerCentroPorId(int id)
    {
        return _centros.Find(n => n.CentroId == id);
    }

    public Centro InsertarCentro(Centro centro)
    {
        Centro? igual = ObtenerCentroPorId(centro.CentroId);
        
        if (igual == null)
        {
            _centros.Add(centro);
            return centro;
        }
        throw new InvalidOperationException("El centro ya existe");
    }

    public Centro ModificarCentro(Centro centro)
    {
        Centro? igual = ObtenerCentroPorId(centro.CentroId);
        
        if (igual == null)
        {
            throw new InvalidOperationException("El centro no existe");
        }

        var index = _centros.IndexOf(igual);
        _centros.Remove(igual);
        _centros.IndexOf(centro, index);
        return centro;
    }

    public bool BorrarCentro(int id)
    {
        var entidad = ObtenerCentroPorId(id);
        if (entidad == null) throw new InvalidOperationException("El centro no existe");
        
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
        
        return _centros.Remove(entidad);
    }
}