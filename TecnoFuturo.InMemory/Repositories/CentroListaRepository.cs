using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
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
    
    public IReadOnlyList<CentroDTO> ObtenerCentros()
    {
        return _centros.Select(x => ToMap(x)).ToList();
    }

    public CentroDTO? ObtenerCentroPorId(int id)
    {
        Centro? centro = _centros.Find(n => n.CentroId == id);
        if (centro != null) return ToMap(centro);
        return null;
    }

    public CentroDTO InsertarCentro(Centro centro)
    {
        CentroDTO? igual = ObtenerCentroPorId(centro.CentroId);
        
        if (igual == null)
        {
            _centros.Add(centro);
            return ToMap(centro);
        }
        throw new InvalidOperationException("El centro ya existe");
    }

    public CentroDTO ModificarCentro(Centro centro)
    {
        CentroDTO? igualDto = ObtenerCentroPorId(centro.CentroId);
        if (igualDto == null) throw new InvalidOperationException("El centro no existe");

        Centro igual = ToEntity(igualDto);

        var index = _centros.IndexOf(igual);
        _centros.Remove(igual);
        var indexOf = _centros.IndexOf(centro, index);

        return ToMap(centro);
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
        
        return _centros.Remove(ToEntity(entidad));
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

    private Centro ToEntity(CentroDTO a)
    {
        return new Centro
        {
            CentroId = a.CentroId,
            Direccion = a.Direccion,
            Nombre = a.Nombre,
            Telefono = a.Telefono
        };
    }
}