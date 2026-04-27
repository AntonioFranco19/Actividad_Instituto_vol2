namespace TecnoFuturo.Core.DTOs;

public record ModuloDTO(
    string CicloFormativoId,
    int ModuloId,
    string? Nombre,
    int Horas,
    string? ProfesorNif
)
{
    public override string ToString()
    {
        return $"""
                CODIGO .: {ModuloId}
                NOMBRE .: {Nombre}
                HORAS ..: {Horas:N0}
                """;
    }
}