using Microsoft.Extensions.DependencyInjection;
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
    
    public IReadOnlyList<Modulo> ObtenerModulos()
    {
        return _modulos.Values.ToList();
    }

    public IReadOnlyList<Modulo> ObtenerModulosPorCicloFormativo(string cicloFormativoId)
    {
        return _modulos.Values.Where(m => m.CicloFormativoId == cicloFormativoId).ToList();
    }

    public IReadOnlyList<Modulo> ObtenerModulosPorProfesor(string profesorNif)
    {
        return _modulos.Values.Where(m => m.ProfesorNif == profesorNif).ToList();
    }

    public Modulo? ObtenerModuloPorId(int id)
    {
        return _modulos.GetValueOrDefault(id);
    }

    public Modulo InsertarModulo(Modulo modulo)
    {
        var cicloFormativoRepository = _serviceProvider.GetRequiredService<ICicloFormativoRepository>();
        var cicloFormativo = cicloFormativoRepository.ObtenerCicloFormativoPorId(modulo.CicloFormativoId);
        
        if (cicloFormativo == null)
        {
            throw new ArgumentException("El ciclo formativo no existe", nameof(modulo.CicloFormativoId));
        }

        return _modulos.TryAdd(modulo.ModuloId, modulo)
            ? modulo
            : throw new InvalidOperationException("El modulo ya existe");
    }

    public Modulo ModificarModulo(Modulo modulo)
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
        
        return _modulos[modulo.ModuloId] = modulo;
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
}