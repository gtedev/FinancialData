namespace FinancialData.Storage;

using MarketStack;
using MongoDB.Driver;

public class FinancialDataStorageClient
{
    private IMongoCollection<EndOfDayMongoDbItem> financialDataCollection;

    public FinancialDataStorageClient(string connectionString, string databaseName)
    {
        var mongoDbClient = new MongoClient(connectionString);

        financialDataCollection =
                mongoDbClient
                    .GetDatabase(databaseName)
                    .GetCollection<EndOfDayMongoDbItem>("FinancialData");
    }

    public async Task InsertEndOfDayItems(IEnumerable<EndOfDayItem> eodItems)
    {
        var items = eodItems.Select(r => new EndOfDayMongoDbItem
        {
            open = r.open,
            high = r.high,
            low = r.low,
            close = r.close,
            volume = r.volume,
            adj_high = r.adj_high,
            adj_low = r.adj_low,
            adj_close = r.adj_close,
            adj_open = r.adj_open,
            adj_volume = r.adj_volume,
            split_factor = r.split_factor,
            dividend = r.dividend,
            symbol = r.symbol,
            exchange = r.exchange,
            date = r.date,
        });

        await financialDataCollection.InsertManyAsync(items);
    }

    public async Task<IEnumerable<EndOfDayItem>> GetEndOfDayItems()
    {
        var items = await financialDataCollection
            .AsQueryable()
            .ToListAsync();

        return items.Select(r => new EndOfDayItem
        {
            open = r.open,
            high = r.high,
            low = r.low,
            close = r.close,
            volume = r.volume,
            adj_high = r.adj_high,
            adj_low = r.adj_low,
            adj_close = r.adj_close,
            adj_open = r.adj_open,
            adj_volume = r.adj_volume,
            split_factor = r.split_factor,
            dividend = r.dividend,
            symbol = r.symbol,
            exchange = r.exchange,
            date = r.date,
        });
    }
}