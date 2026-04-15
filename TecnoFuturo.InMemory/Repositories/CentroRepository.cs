using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.InMemory.Repositories;

public class CentroRepository : ICentroRepository
{
    private readonly Dictionary<int, Centro> _centros = [];
    private readonly IServiceProvider _serviceProvider;

    public CentroRepository(IServiceProvider serviceProvider)
    {
       _serviceProvider = serviceProvider;
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
        return !_centros.TryAdd(centro.CentroId, centro) ? throw new InvalidOperationException("El centro ya existe") : ToMap(centro);
    }

    public CentroDTO ModificarCentro(Centro centro)
    {
        if (!_centros.ContainsKey(centro.CentroId))
        {
            throw new InvalidOperationException("El centro no existe");
        }

        _centros[centro.CentroId] = centro;
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
        
        return _centros.Remove(id);
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