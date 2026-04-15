using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.InMemory.Repositories;

public class ModuloRepository : IModuloRepository
{
    private IServiceProvider _serviceProvider;
    private readonly Dictionary<int, Modulo> _modulos = [];
    
    public ModuloRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IReadOnlyList<ModuloDTO> ObtenerModulos()
    {
        return _modulos.Values.Select(x => ToMap(x)).ToList();
    }

    public IReadOnlyList<ModuloDTO> ObtenerModulosPorCicloFormativo(string cicloFormativoId)
    {
        return _modulos.Values.Where(m => m.CicloFormativoId == cicloFormativoId).Select(x => ToMap(x)).ToList();
    }

    public IReadOnlyList<ModuloDTO> ObtenerModulosPorProfesor(string profesorNif)
    {
        return _modulos.Values.Where(m => m.ProfesorNif == profesorNif).Select(x=> ToMap(x)).ToList();
    }

    public ModuloDTO? ObtenerModuloPorId(int id)
    {
        var modulo = _modulos.GetValueOrDefault(id);
        if (modulo != null) return ToMap(modulo);
        return null;
    }

    public ModuloDTO InsertarModulo(Modulo modulo)
    {
        var cicloFormativoRepository = _serviceProvider.GetRequiredService<ICicloFormativoRepository>();
        var cicloFormativo = cicloFormativoRepository.ObtenerCicloFormativoPorId(modulo.CicloFormativoId);
        
        if (cicloFormativo == null)
        {
            throw new ArgumentException("El ciclo formativo no existe", nameof(modulo.CicloFormativoId));
        }

        return _modulos.TryAdd(modulo.ModuloId, modulo)
            ? ToMap(modulo)
            : throw new InvalidOperationException("El modulo ya existe");
    }

    public ModuloDTO ModificarModulo(Modulo modulo)
    {
        var cicloFormativoRepository = _serviceProvider.GetRequiredService<ICicloFormativoRepository>();
        var cicloFormativo = cicloFormativoRepository.ObtenerCicloFormativoPorId(modulo.CicloFormativoId);
        
        if (cicloFormativo == null)
        {
            throw new ArgumentException("El ciclo formativo no existe", nameof(modulo.CicloFormativoId));
        }

        if (!_modulos.ContainsKey(modulo.ModuloId))
        {
            throw new ArgumentException("El modulo no existe", nameof(modulo.ModuloId));
        }
        
        return ToMap(_modulos[modulo.ModuloId] = modulo);
    }

    public bool BorrarModulo(int id)
    {
        if (_modulos.TryGetValue(id, out var modulo))
        {
            var cicloFormativoRepository = _serviceProvider.GetRequiredService<ICicloFormativoRepository>();
            var cicloFormativo = cicloFormativoRepository.ObtenerCicloFormativoPorId(modulo.CicloFormativoId);
        
            if (cicloFormativo == null)
            {
                throw new ArgumentException("El ciclo formativo no existe", nameof(modulo.CicloFormativoId));
            }
            
            return _modulos.Remove(id);
        }
        return false;
    }
    
    private ModuloDTO ToMap(Modulo a)
    {
        return new ModuloDTO(
        CicloFormativoId: a.CicloFormativoId,
        ModuloId: a.ModuloId,
        Nombre: a.Nombre,
        Horas: a.Horas,
        ProfesorNif: a.ProfesorNif
            );
    }
}