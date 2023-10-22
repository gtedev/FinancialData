using System.Dynamic;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace FinancialData.Storage;

using MarketStack;
using MongoDB.Driver;


public class FinancialDataStorageClient
{
    private IMongoCollection<BsonDocument> financialDataCollection;

    public FinancialDataStorageClient(string connectionString, string databaseName)
    {
        var mongoDbClient = new MongoClient(connectionString);

        financialDataCollection =
                mongoDbClient
                    .GetDatabase(databaseName)
                    .GetCollection<BsonDocument>("FinancialData");
    }

    public async Task InsertEndOfDayItems(IEnumerable<EndOfDayItem> eodItems)
    {
        var items = eodItems.Select(r => r.ToBsonDocument());

        await financialDataCollection.InsertManyAsync(items);
    }

    public async Task<IEnumerable<EndOfDayItem>> GetEndOfDayItems()
    {
        var items = await financialDataCollection
            .AsQueryable()
            .ToListAsync();

        return items.Select(r => new EndOfDayItem
        {
            open = r.Field("open").AsDouble,
            high = r.Field("high").AsDouble,
            low = r.Field("low").AsDouble,
            close = r.Field("close").AsDouble,
            volume = r.Field("volume").AsDouble,
            adj_high = r.Field("adj_high").AsDouble,
            adj_low = r.Field("adj_low").AsDouble,
            adj_close = r.Field("adj_close").AsDouble,
            adj_open = r.Field("adj_open").AsDouble,
            adj_volume = r.Field("adj_volume").AsDouble,
            split_factor = r.Field("split_factor").AsDouble,
            dividend = r.Field("dividend").AsDouble,
            symbol = r.Field("symbol").AsString,
            exchange = r.Field("exchange").AsString,
            date = r.Field("date").AsString
        });
    }
}