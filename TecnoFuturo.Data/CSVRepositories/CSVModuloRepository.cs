using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Core.Validators;

namespace TecnoFuturo.Data.CSVRepositories;

public class CsvModuloRepository : IModuloRepository
{
    private IServiceProvider _serviceProvider;
    private Dictionary<int, Modulo> _modulos;
    private const string SaveFile = "modulos.csv";
    
    public CsvModuloRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CargarDatos();
    }

    private void GuardarEnArchivo()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(SaveFile, FileMode.OpenOrCreate)))
            {
                foreach (var value in _modulos.Values)
                {
                    writer.WriteLine($"{value.CicloFormativoId};{value.ModuloId};{value.Nombre};{value.Horas};{value.ProfesorNif}");
                }
            }
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
                _modulos = new Dictionary<int, Modulo>();
                return;
            }

            using (StreamReader reader = new StreamReader(new FileStream(SaveFile, FileMode.Open)))
            {
                _modulos = new Dictionary<int, Modulo>();
                string? linea;
                while (string.IsNullOrWhiteSpace(linea = reader.ReadLine()))
                {
                    Modulo? modulo = ConvertirStringAModulo(linea);
                    if (modulo != null) _modulos.TryAdd(modulo.ModuloId, modulo);
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido cargar los datos");
            throw;
        }
    }
    
    private Modulo? ConvertirStringAModulo(string? cadena)
    {
        if (cadena == null) return null;
        if (!Regex.IsMatch(cadena, @"^([^;]+);([^;]+);([^;]+);([^;]+);([^;]+)$")) return null;
        
        string[] campos = cadena.Split(';');

        if (campos.Length != 5)
        {
            return null;
        }

        if (!int.TryParse(campos[1], out int moduloId))
        {
            moduloId = 0;
        }
        
        if (!int.TryParse(campos[3], out int horas))
        {
            horas = 0;
        }
        
        var modulo = new Modulo
        {
            CicloFormativoId = campos[0],
            ModuloId = moduloId,
            Nombre = campos[2],
            Horas = horas,
            ProfesorNif = campos[4]
        };

        ModuloValidator validator = new ModuloValidator();

        return validator.Validate(modulo) ? modulo : null;
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