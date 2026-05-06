using MessagePack;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TecnoFuturo.Core.Helpers;

namespace TecnoFuturo.Core.Entities;
// [MessagePackObject]
// [Union(1, typeof(Alumno))]
// [Union(2, typeof(Profesor))]

public abstract class Persona : IInfoDetallada
{
    [BsonId]
    public ObjectId Id { get; set; }
    // [Key(0)]
    [BsonElement("Nif")]
    public string Nif { get; set; } = null!;
    [BsonElement("Nombre")]
    public string Nombre { get; set; } = null!;
    // [Key(2)]
    [BsonElement("Email")]
    public string? Email { get; set; }
    // [Key(3)]
    [BsonElement("Direccion")]
    public string? Direccíon { get; set; }
    // [Key(4)]
    [BsonElement("Telefono")]
    public string? Telefono { get; set; }
    public virtual string ObtenerFicha()
    {
        return " ";
    }
}