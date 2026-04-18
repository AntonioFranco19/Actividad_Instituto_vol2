using MessagePack;
using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;

[MessagePackObject]
public class Modulo : IInfoDetallada
{
    [Key(0)] public string CicloFormativoId { get; set; } = null!;
    [Key(1)] public int ModuloId { get; set; }
    [Key(2)] public string? Nombre { get; set; }
    [Key(3)] public int Horas { get; set; }
    [Key(4)] public string? ProfesorNif { get; set; }

    public string ObtenerFicha()
    {
        return $"""
                CODIGO .: {ModuloId}
                NOMBRE .: {Nombre}
                HORAS ..: {Horas:N0}
                """;
    }
}