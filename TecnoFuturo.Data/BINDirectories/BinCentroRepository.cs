using System.Text.Json;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.Data.BINDirectories;

public class BinCentroRepository : ICentroRepository
{
    private Dictionary<int, Centro> _centros;
    private readonly IServiceProvider _serviceProvider;
    private const string SaveFile = "centros.bin";

    public BinCentroRepository(IServiceProvider serviceProvider)
    {
       _serviceProvider = serviceProvider;
       CargarDatos();
    }
    
    private void GuardarEnArchivo()
    {
        try
        {
            byte[] datos = MessagePackSerializer.Serialize(_centros.Values);
            File.WriteAllBytes(SaveFile, datos);

        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido guardar los datos");
            throw;
        }
    }

    private void CargarDatos()
    {
        try
        {
            if (!File.Exists(SaveFile))
            {
                _centros = new Dictionary<int, Centro>();
                return;
            }

            byte[] datos = File.ReadAllBytes(SaveFile);
            var lista = MessagePackSerializer.Deserialize<List<Centro>>(datos);
            
            _centros = lista.ToDictionary(x => x.CentroId);
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido cargar los datos");
            throw;
        }
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
        bool exito = _centros.TryAdd(centro.CentroId, centro);
        if (exito)
        {
            GuardarEnArchivo();
            return ToMap(centro);
        }
        throw new InvalidOperationException("El centro ya existe");
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