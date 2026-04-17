using MessagePack;

namespace TecnoFuturo.Core.Entities;
[MessagePackObject]
public class Alumno : Persona
{
    [Key(5)]
    public int CentroId { get; set; }
    [Key(6)]
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