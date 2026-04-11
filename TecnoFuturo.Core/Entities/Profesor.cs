namespace TecnoFuturo.Core.Entities;

public class Profesor : Persona
{
    public int CentroId { get; set; }

    public override string ObtenerFicha()
    {
        return $"""
               NOMBRE: {Nombre}
               NIF   : {Nif}"
               EMAIL : {Email}
               {new string('-', 85)}
               """;
    }
}