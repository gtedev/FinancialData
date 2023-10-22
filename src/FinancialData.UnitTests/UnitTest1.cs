using FinancialData.Storage;
using FinancialData.MarketStack;
using Xunit;
using MongoDB.Driver;
using System.Dynamic;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace FinancialData.UnitTests;

public class UnitTest1
{
    const string accessKey = "<Put here your Access Key>";
    const string mongoDbConnectionString = "<Put here your Connection string>";
    const string databaseName = "<Put here your Database name>";

    MarksetStackClient marketStackClient;
    FinancialDataStorageClient storageClient;

    public UnitTest1()
    {
        marketStackClient = new MarksetStackClient(accessKey, new HttpClient());
        storageClient = new FinancialDataStorageClient(mongoDbConnectionString, databaseName);
    }

    private async Task Insert(int pageNumber, int offset, int accumulatedCount, int? total)
    {
        // In case we run in infinite loop
        // We limit number of recursive query to 500.
        if (pageNumber == 500 || accumulatedCount == total)
        {
            return;
        }

        var response = await marketStackClient.GetEndOfDayAsync(offset, new List<string> { "DOW" });
        await storageClient.InsertEndOfDayItems(response.data);

        await Insert(
            pageNumber: pageNumber + 1,
            offset: response.pagination.offset + response.pagination.limit,
            accumulatedCount: accumulatedCount + response.data.Count,
            total: response.pagination.total);
    }

    [Fact]
    public async Task InsertEndOfDayDataFromMarketStack()
    {
        await Insert(
                pageNumber: 1,
                offset: 0,
                accumulatedCount: 0,
                total: null);
    }

    [Fact]
    public async Task GetEndOfDataFromStorage()
    {
        var item = await storageClient.GetEndOfDayItems();
    }

    public class Toto
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
    }

    Toto toto(string d)
    {
        return new Toto
        {
           A = "A", 
           B = "B", 
           C = "C", 
           D = d
        };
    }
    

    [Fact] public async Task InsertWithoutDuplicate()
    {

        var mongoDbClient = new MongoClient(mongoDbConnectionString);

        var items = new List<Toto>
        {
            toto("aaa"),
            toto("bbb"),
            toto("ccc"),
            toto("ddd")
        };
        
        var collection =
            mongoDbClient
                .GetDatabase(databaseName)
                .GetCollection<Toto>("Gerard");
        
        var operations = items.Select(x =>
        {
            // Define fields that would make field unique
            var filter = Builders<Toto>.Filter.And(
                Builders<Toto>.Filter.Eq(a => a.A, x.A),
                Builders<Toto>.Filter.Eq(a => a.B, x.B),
                Builders<Toto>.Filter.Eq(a => a.C, x.C)
            );
            
            var upsert = new ReplaceOneModel<Toto>(filter, x)
            {
                IsUpsert = true
            };

            return upsert;
        });
        
        await collection.BulkWriteAsync(operations) ;
    }
}