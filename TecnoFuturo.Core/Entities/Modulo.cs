using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;

public class Modulo : IInfoDetallada
{
    public string CicloFormativoId { get; set; } = null!;
    public int ModuloId { get; set; }
    public string? Nombre { get; set; }
    public int Horas { get; set; }
    public string? ProfesorNif { get; set; }

    public string ObtenerFicha()
    {
        return $"""
                 CODIGO .: {ModuloId}
                 NOMBRE .: {Nombre}
                 HORAS ..: {Horas:N0}
                 """;
    }
}