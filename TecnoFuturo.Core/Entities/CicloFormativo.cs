using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;

public class CicloFormativo : IInfoDetallada
{
    public int CentroId { get; set; }
    public string CicloFormativoId { get; set; } = null!;
    public string Nombre { get; set; } = null!;
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