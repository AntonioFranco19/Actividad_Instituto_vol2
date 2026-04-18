using System.Text.Json;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using TecnoFuturo.Core.DTOs;
using TecnoFuturo.Core.Entities;
using TecnoFuturo.Core.Repositories;

namespace TecnoFuturo.Data.BINDirectories;

public class BinCicloFormativoRepository
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<string, CicloFormativo> _ciclosFormativos;
    private const string SaveFile = "ciclosformativos.bin";


    public BinCicloFormativoRepository(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CargarDatos();
    }

    private void GuardarEnArchivo()
    {
        try
        {
            byte[] datos = MessagePackSerializer.Serialize(_ciclosFormativos.Values);
            File.WriteAllBytes(SaveFile, datos);
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

            byte[] datos = File.ReadAllBytes(SaveFile);
            var lista = MessagePackSerializer.Deserialize<List<CicloFormativo>>(datos);

            _ciclosFormativos = lista.ToDictionary(x => x.CicloFormativoId);
        }
        catch (Exception)
        {
            Console.WriteLine("No se han podido cargar los datos.");
            throw;
        }
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