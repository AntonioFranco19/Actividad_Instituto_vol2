using MessagePack;
using MongoDB.Bson.Serialization.Attributes;

namespace TecnoFuturo.Core.Entities;

// [MessagePackObject]
public class Profesor : Persona
{
   // [Key(5)]
   [BsonElement("Centro_Id")] 
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