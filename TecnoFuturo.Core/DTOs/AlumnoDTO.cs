namespace TecnoFuturo.Core.DTOs;

public record AlumnoDTO(
    string Nif,
    string Nombre,
    string? Email,
    string? Direccion,
    string? Telefono,
    int CentroId,
    string CicloFormativoId
)
{
    public override string ToString()
    {
        return $"""
                NIF ......: {Nif}
                NOMBRE  ..: {Nombre}
                EMAIL ....: {Email}
                DIRECCION : {Direccion}
                {new string('-', 102)}
                """;
    }
}