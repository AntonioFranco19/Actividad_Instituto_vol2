using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Data.Helpers;

namespace TecnoFuturo.Data.Repsoitories;

public class JsonModuloRepository : IModuloRepository
{
    private readonly JsonHelper _jsonHelper;
    private IServiceProvider _serviceProvider;
    private Dictionary<int, Modulo> _modulos;
    private readonly string _saveFile;
    
    public JsonModuloRepository(DataConfig.DataConfig dataConfig, JsonHelper jsonHelper, IServiceProvider serviceProvider)
    {
        _jsonHelper = jsonHelper;
        _serviceProvider = serviceProvider;
        _saveFile = dataConfig.GetSecureFilePath("modulos.json");
        CargarDatos();
    }

    private void GuardarEnArchivo()
    {
        _jsonHelper.GuardarDatos(_saveFile, _modulos.Values);
    }

    private void CargarDatos()
    {
        var modulos = _jsonHelper.LeerDatos<Modulo>(_saveFile);
        if (modulos == null)
        {
            _modulos = new Dictionary<int, Modulo>();
            return;
        }
        _modulos = modulos.ToDictionary(m=> m.ModuloId);
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

        if (_modulos.TryAdd(modulo.ModuloId, modulo))
        {
            GuardarEnArchivo();
            return ToMap(modulo);
        }
        throw new InvalidOperationException("El modulo ya existe");
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
        
        _modulos[modulo.ModuloId] = modulo;
        GuardarEnArchivo();
        return ToMap(modulo);
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
            
            bool exito = _modulos.Remove(id);
            if (exito) GuardarEnArchivo();
            return exito;
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