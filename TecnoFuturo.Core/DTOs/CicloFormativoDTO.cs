using TecnoFuturo.Core.Entities;

namespace TecnoFuturo.Core.DTOs;

public record CicloFormativoDTO(
    int CentroId,
    string CicloFormativoId,
    string Nombre,
    Turno Turno
)
{
    public override string ToString()
    {
        return $"""
                CODIGO : {CicloFormativoId}
                NOMBRE : {Nombre}
                TURNO ..: {Turno}
                {new string('-', 85)}
                """;
    }
}