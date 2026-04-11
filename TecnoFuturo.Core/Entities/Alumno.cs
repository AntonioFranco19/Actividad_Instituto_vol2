namespace TecnoFuturo.Core.Entities;

public class Alumno : Persona
{
    public int CentroId { get; set; }
    public string CicloFormativoId { get; set; } = null!;

    public override string ObtenerFicha()
    {
        return $"""
               NIF ......: {Nif}
               NOMBRE  ..: {Nombre}
               EMAIL ....: {Email}
               DIRECCION : {Direccíon}
               {new string('-', 102)}
               """;
    }
}