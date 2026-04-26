namespace TecnoFuturo.Core.DTOs;

public record ProfesorDTO(
    string Nif,
    string Nombre,
    string? Email,
    string? Direccion,
    string? Telefono,
    int CentroId
)
{
    public override string ToString()
    {
        return $"""
                NOMBRE: {Nombre}
                NIF   : {Nif}"
                EMAIL : {Email}
                {new string('-', 85)}
                """;
    }
}