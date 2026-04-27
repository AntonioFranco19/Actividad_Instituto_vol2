using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Data.Helpers;

namespace TecnoFuturo.Data.JSONRepositories;

public class JsonCentroRepository : ICentroRepository
{
    private readonly JsonHelper _jsonHelper;
    private Dictionary<int, Centro> _centros;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _saveFile;

    public JsonCentroRepository(DataConfig.DataConfig dataConfig, JsonHelper jsonHelper, IServiceProvider serviceProvider)
    {
        _jsonHelper = jsonHelper;
       _serviceProvider = serviceProvider;
       _saveFile = dataConfig.GetSecureFilePath("centros.json");
       CargarDatos();
    }
    
    private void GuardarEnArchivo()
    {
        _jsonHelper.GuardarDatos(_saveFile, _centros.Values);
    }

    private void CargarDatos()
    {
        var centros = _jsonHelper.LeerDatos<Centro>(_saveFile);
        if (centros == null)
        {
            _centros = new Dictionary<int, Centro>();
            return;
        }

        _centros = centros.ToDictionary(c => c.CentroId);
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
        _centros.TryAdd(centro.CentroId, centro);
        GuardarEnArchivo();
        return ToMap(centro);
    }

    public CentroDTO ModificarCentro(Centro centro)
    {
        if (!_centros.ContainsKey(centro.CentroId))
        {
            throw new InvalidOperationException("El centro no existe");
        }

        _centros[centro.CentroId] = centro;
        GuardarEnArchivo();
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
        
        bool exito = _centros.Remove(id);
        if (exito) GuardarEnArchivo();
        return exito;
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