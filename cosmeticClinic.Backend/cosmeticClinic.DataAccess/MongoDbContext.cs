using cosmeticClinic.Entities;
using cosmeticClinic.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace cosmeticClinic.DataAccess;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> mongoDBSettings)
    {
        var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
        _database = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
    }

}