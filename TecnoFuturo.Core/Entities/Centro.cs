using MessagePack;
using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;
[MessagePackObject]
public class Centro : IInfoDetallada
{
    [Key(0)]
    public int CentroId { get; set; }
    [Key(1)]
    public string Nombre { get; set; } = null!;
    [Key(2)]
    public string? Direccion { get; set; }
    [Key(3)]
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