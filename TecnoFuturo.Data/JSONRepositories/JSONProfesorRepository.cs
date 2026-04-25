using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Data.Helpers;

namespace TecnoFuturo.Data.Repsoitories;

public class JsonProfesorRepository : IProfesorRepository
{
    private JsonHelper _jsonHelper;
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, Profesor> _profesores;
    private readonly string _saveFile = DataConfig.GetFilePath("profesores.json");
    
    public JsonProfesorRepository(JsonHelper jsonHelper, IServiceProvider serviceProvider)
    {
        _jsonHelper = jsonHelper;
        _serviceProvider = serviceProvider;
        CargarDesdeArchivo();
    }

    private void GuardarEnArchivo()
    {
        _jsonHelper.GuardarDatos(_saveFile, _profesores.Values);
    }

    private void CargarDesdeArchivo()
    {
        var profesores = _jsonHelper.LeerDatos<Profesor>(_saveFile);

        if (profesores == null)
        {
            _profesores = new Dictionary<string, Profesor>();
            return;
        }

        _profesores = profesores.ToDictionary(p => p.Nif);
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
        
        _profesores[profesor.Nif] = profesor;
        GuardarEnArchivo();
        return ToMap(profesor);
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
        
        _profesores[profesor.Nif] = profesor;
        GuardarEnArchivo();
        return ToMap(profesor);
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
        if (modulosPorProfesor.Count != 0) throw new InvalidOperationException("El profesor tiene modulos asignados");

        bool exito = _profesores.Remove(nif);
        if (exito) GuardarEnArchivo();
        return exito;
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