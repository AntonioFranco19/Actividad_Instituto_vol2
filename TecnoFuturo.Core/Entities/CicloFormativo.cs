using MessagePack;
using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;
[MessagePackObject]
public class CicloFormativo : IInfoDetallada
{
    [Key(0)]
    public int CentroId { get; set; }
    [Key(1)]
    public string CicloFormativoId { get; set; } = null!;
    [Key(2)]
    public string Nombre { get; set; } = null!;
    [Key(3)]
    public Turno Turno { get; set; }
    public string ObtenerFicha()
    {
        return $"""
                 CODIGO : {CicloFormativoId}
                 NOMBRE : {Nombre}
                 TURNO ..: {Turno}
                 {new string('-', 85)}
                 """;
    }
}