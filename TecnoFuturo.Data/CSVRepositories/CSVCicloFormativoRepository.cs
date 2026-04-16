using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;
using TecnoFuturo.Core.Validators;

namespace TecnoFuturo.Data.CSVRepositories;

public class CsvCicloFormativoRepository : ICicloFormativoRepository
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, CicloFormativo> _ciclosFormativos;
    private const string SaveFile = "ciclosformativos.csv";
    
    
    public CsvCicloFormativoRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CargarDatos();
    }

    private void GuardarEnArchivo()
    {
        try
        {
            using (FileStream fs = new FileStream(SaveFile, FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (var value in _ciclosFormativos.Values)
                    {
                        writer.WriteLine($"{value.CentroId};{value.CicloFormativoId};{value.Nombre};{value.Turno}");
                    }
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido guardar los datos.");
            throw;
        }
        
    }

    private void CargarDatos()
    {
        try
        {
            if (!File.Exists(SaveFile))
            {
                _ciclosFormativos = new Dictionary<string, CicloFormativo>();
                return;
            }

            using (FileStream fs = new FileStream(SaveFile, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    _ciclosFormativos = new Dictionary<string, CicloFormativo>();
                    string? linea;
                    while (string.IsNullOrWhiteSpace(linea = reader.ReadLine()))
                    {
                        CicloFormativo? ciclo = ConvertirStringACiclo(linea);
                        if (ciclo != null)
                        {
                            _ciclosFormativos.TryAdd(ciclo.CicloFormativoId, ciclo);   
                        }
                    }
                }
            }
            
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido cargar los datos.");
            throw;
        }
    }

    private CicloFormativo? ConvertirStringACiclo(string? cadena)
    {
        if (cadena == null) return null;
        if (!Regex.IsMatch(cadena, @"^([^;]+);([^;]+);([^;]+);([^;]+)$")) return null;

        string[] datos = cadena.Split(';');

        if (datos.Length != 5)
        {
            return null;
        }

        if (!int.TryParse(datos[0], out int centroId))
        {
            centroId = 0;
        }

        Turno turno = datos[3] switch
        {
            "Matutino" => Turno.Matutino,
            "Vespertino" => Turno.Vespertino,
            _ => Turno.Nocturno
        };

        var cicloFormativo = new CicloFormativo
        {
            CentroId = centroId,
            CicloFormativoId = datos[1],
            Nombre = datos[2],
            Turno = turno
        };

        var valitador = new CicloFormativoValidator();

        return valitador.Validate(cicloFormativo) ? cicloFormativo : null;
    }
    
    public IReadOnlyList<CicloFormativoDTO> ObtenerCiclosFormativos()
    {
        return _ciclosFormativos.Values.Select(x => ToMap(x)).ToList();
    }

    public IReadOnlyList<CicloFormativoDTO> ObtenerCiclosFormativosPorCentro(int centroId)
    {
        return _ciclosFormativos.Values.Where(c => c.CentroId == centroId).Select(x => ToMap(x)).ToList();
    }

    public CicloFormativoDTO? ObtenerCicloFormativoPorId(string id)
    {
        var ciclo = _ciclosFormativos.GetValueOrDefault(id);
        if (ciclo != null) return ToMap(ciclo);
        return null;
    }

    public CicloFormativoDTO InsertarCicloFormativo(CicloFormativo cicloFormativo)
    {
        var centroRepository = _serviceProvider.GetRequiredService<ICentroRepository>();
        var centro = centroRepository.ObtenerCentroPorId(cicloFormativo.CentroId);
        if (centro == null)
        {
            throw new ArgumentException("El centro especificado no existe", nameof(cicloFormativo));
        }
        
        if (_ciclosFormativos.TryAdd(cicloFormativo.CicloFormativoId, cicloFormativo))
        {
            GuardarEnArchivo();
            return ToMap(cicloFormativo);
        }

        throw new InvalidOperationException("El ciclo formativo ya existe");
    }

    public CicloFormativoDTO ModificarCicloFormativo(CicloFormativo cicloFormativo)
    {
        var centroRepository = _serviceProvider.GetRequiredService<ICentroRepository>();
        var centro = centroRepository.ObtenerCentroPorId(cicloFormativo.CentroId);
        if (centro == null)
        {
            throw new ArgumentException("El centro especificado no existe", nameof(cicloFormativo));
        }
        
        if (!_ciclosFormativos.ContainsKey(cicloFormativo.CicloFormativoId))
        {
            throw new ArgumentException("El ciclo formativo no existe", nameof(cicloFormativo));
        }
        
        _ciclosFormativos[cicloFormativo.CicloFormativoId] = cicloFormativo;
        GuardarEnArchivo();
        return ToMap(cicloFormativo);
    }

    public bool BorrarCicloFormativo(string id)
    {
        if (!_ciclosFormativos.TryGetValue(id, out var cicloFormativo)) return false;
        
        var centroRepository = _serviceProvider.GetRequiredService<ICentroRepository>();
        var centro = centroRepository.ObtenerCentroPorId(cicloFormativo.CentroId);
        if (centro == null)
        {
            throw new ArgumentException("El centro especificado no existe");
        }

        var moduloRepository = _serviceProvider.GetRequiredService<IModuloRepository>();
        var modulos = moduloRepository.ObtenerModulosPorCicloFormativo(id);
        
        if (modulos.Count != 0)
        {
            throw new InvalidOperationException("El ciclo formativo tiene modulos asociados");
        }
        
        bool exito = _ciclosFormativos.Remove(id);
        if (exito) GuardarEnArchivo();
        return exito;
    }
    
    private CicloFormativoDTO ToMap(CicloFormativo a)
    {
        return new CicloFormativoDTO(
            CentroId: a.CentroId, 
            CicloFormativoId: a.CicloFormativoId,
            Nombre: a.Nombre, 
            Turno: a.Turno
        );
    }
    
}