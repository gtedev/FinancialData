namespace FinancialData.Storage;

using MarketStack;
using MongoDB.Driver;

public class FinancialDataStorageClient
{
    private IMongoCollection<EndOfDayItem> insertCollection;
    private IMongoCollection<EndOfDayMongoDbItem> readCollection;
    private const string collectioName = "FinancialData";
        
    public FinancialDataStorageClient(string connectionString, string databaseName)
    {
        
        var mongoDbClient = new MongoClient(connectionString);

        insertCollection =
                mongoDbClient
                    .GetDatabase(databaseName)
                    .GetCollection<EndOfDayItem>(collectioName);
        
        readCollection =
            mongoDbClient
                .GetDatabase(databaseName)
                .GetCollection<EndOfDayMongoDbItem>(collectioName);
      
    }
    
    private ReplaceOneModel<EndOfDayItem> createUpSert(EndOfDayItem x)
    {
        // Define fields that makes unique a record
        // If one entry is found with same exchange, symbol and date
        // It will be overwritten, otherwise, it will be just inserted.
            var filter = Builders<EndOfDayItem>.Filter.And(
                Builders<EndOfDayItem>.Filter.Eq(f => f.exchange, x.exchange),
                Builders<EndOfDayItem>.Filter.Eq(f => f.symbol, x.symbol),
                Builders<EndOfDayItem>.Filter.Eq(f => f.date, x.date)
            );
            
            return new ReplaceOneModel<EndOfDayItem>(filter, x)
            {
                IsUpsert = true
            };;
    }


    
    public async Task InsertEndOfDayItems(IEnumerable<EndOfDayItem> eodItems)
    {
        
        var upSertOperations = eodItems.Select(createUpSert);

        await insertCollection.BulkWriteAsync(upSertOperations);
    }

    public async Task<IEnumerable<EndOfDayItem>> GetEndOfDayItems()
    {
        var items = await readCollection
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