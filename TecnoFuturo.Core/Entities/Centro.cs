using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;

public class Centro : IInfoDetallada
{
    public int CentroId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    
    public string ObtenerFicha()
    {
        return $"""
                 -> Centro: {Nombre}[{CentroId}]
                 -> Direccion: {Direccion}
                 -> Telefono: {Telefono}
                """;
    }
}