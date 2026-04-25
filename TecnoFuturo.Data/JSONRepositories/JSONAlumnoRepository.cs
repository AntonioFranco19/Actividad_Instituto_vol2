using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Data.Helpers;

namespace TecnoFuturo.Data.Repsoitories;

public class JsonAlumnoRepository : IAlumnoRepository
{
    private JsonHelper _jsonHelper;
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, Alumno> _alumnos;
    private readonly string _saveFile;

    public JsonAlumnoRepository(DataConfig.DataConfig dataConfig, JsonHelper jsonHelper, IServiceProvider serviceProvider)
    {
        _jsonHelper = jsonHelper;
        _serviceProvider = serviceProvider;
        _saveFile = dataConfig.GetSecureFilePath("alumnos.json");
        CargarDesdeArchivo();
    }

    private void GuardarEnArchivo()
    {
        _jsonHelper.GuardarDatos(_saveFile, _alumnos.Values);
    }

    private void CargarDesdeArchivo()
    {
        var alumnos = _jsonHelper.LeerDatos<Alumno>(_saveFile);
        if (alumnos != null)
        {
            _alumnos = alumnos.ToDictionary(a => a.Nif);
            return;
        }
        _alumnos = new Dictionary<string, Alumno>();
    }

    public IReadOnlyList<AlumnoDTO> ObtenerAlumnos()
    {
        return _alumnos.Values.Select(x => ToMap(x)).ToList();
    }

    public IReadOnlyList<AlumnoDTO> ObtenerAlumnosPorCicloFormativo(string cicloFormativoId)
    {
        return _alumnos.Values.Where(alumno => alumno.CicloFormativoId == cicloFormativoId).Select(x => ToMap(x)).ToList();
    }

    public IReadOnlyList<AlumnoDTO> ObtenerAlumnosPorCentro(int centroId)
    {
        return _alumnos.Values.Where(alumno => alumno.CentroId == centroId).Select(x => ToMap(x)).ToList();
    }

    public AlumnoDTO? ObtenerAlumnoPorNif(string nif)
    {
        Alumno? alumno = _alumnos.GetValueOrDefault(nif);
        if (alumno != null) return ToMap(alumno);
        return null;
    }

    public AlumnoDTO InsertarAlumno(Alumno alumno)
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
        
        
        _alumnos[alumno.Nif] = alumno;
        GuardarEnArchivo();
        return ToMap(alumno);
    }

    public AlumnoDTO ModificarAlumno(Alumno alumno)
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
        
        _alumnos[alumno.Nif] = alumno;
        GuardarEnArchivo();
        return ToMap(alumno);
    }

    public bool BorrarAlumno(string nif)
    {
        if (!_alumnos.ContainsKey(nif))
        {
            return false;
        }
        bool exito = _alumnos.Remove(nif);
        if(exito) GuardarEnArchivo();
        return exito;
    }
    
    private AlumnoDTO ToMap(Alumno a)
    {
        return new AlumnoDTO(
            Nif: a.Nif, 
            Nombre: a.Nombre, 
            Email: a.Email, 
            Direccion: a.Direccíon, 
            Telefono: a.Telefono,
            CentroId: a.CentroId, 
            CicloFormativoId: a.CicloFormativoId);
    }
}