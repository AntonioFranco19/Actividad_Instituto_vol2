using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using TecnoFuturo.Database.Options;

namespace TecnoFuturo.Database.Services;

public class DataBaseService
{
    public MongoClient Client { get; }
    public IMongoDatabase Database { get; }

    // El constructor es privado para evitar el uso de 'new' fuera de aquí
    public DataBaseService()
    {
        // ------------------------------------------------------------------
    // CONFIGURACIÓN GLOBAL BSON (Se ejecuta una sola vez)
    // ------------------------------------------------------------------
    // 1. Configuramos los decimales normales (decimal)
        BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
        // 2. Configuramos los decimales que pueden ser nulos (decimal?)
        BsonSerializer.RegisterSerializer(
            new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128))
        );
// ------------------------------------------------------------------

// En una app real, esto vendría de un archivo de configuración
        string? connectionString = Environment.GetEnvironmentVariable("FINTECHFP_MONGODB");
        string dbName = "FinTechFP";
        Client = new MongoClient(connectionString);
        Database = Client.GetDatabase(dbName);
    }
}