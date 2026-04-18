using System.Text.Json;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.Data.BINDirectories;

public class BinProfesorRepository : IProfesorRepository
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, Profesor> _profesores;
    private const string SaveFile = "profesores.bin";
    
    public BinProfesorRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CargarDesdeArchivo();
    }

    private void GuardarEnArchivo()
    {
        try
        {
            byte[] datos = MessagePackSerializer.Serialize(_profesores.Values);
            File.WriteAllBytes(SaveFile, datos);
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido guardar los datos");
            throw;
        }
    }

    private void CargarDesdeArchivo()
    {
        try
        {
            if (!File.Exists(SaveFile))
            {
                _profesores = new Dictionary<string, Profesor>();
                return;
            }
            
            byte[] datos = File.ReadAllBytes(SaveFile);

            var lista = MessagePackSerializer.Deserialize<List<Profesor>>(datos);
            _profesores = lista!.ToDictionary(x => x.Nif);
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido cargar los archivos");
            throw;
        }
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