
using MessagePack;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TecnoFuturo.Core.Entities;

//[MessagePackObject]
public class Alumno : Persona
{
    //[Key(5)]
    [BsonElement("Centro_Id")]
    public int CentroId { get; set; }
    // [Key(6)]
    [BsonElement("CicloFormativo_Id")]
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