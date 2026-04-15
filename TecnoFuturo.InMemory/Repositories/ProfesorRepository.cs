using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.InMemory.Repositories;

public class ProfesorRepository : IProfesorRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Profesor> _profesores = [];
    
    public ProfesorRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        
        return ToMap(_profesores[profesor.Nif] = profesor);
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
        
        return ToMap(_profesores[profesor.Nif] = profesor);
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
        return modulosPorProfesor.Count != 0
            ? throw new InvalidOperationException("El profesor tiene modulos asignados")
            : _profesores.Remove(nif);
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