using MongoDB.Driver;

namespace Stock.API.Services
{
    public class MongoDBService
    {
        readonly IMongoDatabase database;
        public MongoDBService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            database = client.GetDatabase("StockDB");
        }
        public IMongoCollection<T> GetCollection<T>()
        {
            return database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
        }
    }
}