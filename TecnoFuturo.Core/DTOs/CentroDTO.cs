namespace TecnoFuturo.Core.DTOs;

public record CentroDTO(
    int CentroId,
    string Nombre,
    string? Direccion,
    string? Telefono
)
{
    public override string ToString()
    {
        return $"""
                        -> Centro: {Nombre}[{CentroId}]
                        -> Direccion: {Direccion}
                        -> Telefono: {Telefono}
                       """;
    }
}