using FinancialData.Storage;
using FinancialData.MarketStack;
using Xunit;

namespace FinancialData.UnitTests;

public class UnitTest1
{
    const string accessKey = "<Put your Access Key here>";
    const string mongoDbConnectionString = "mongodb://localhost:27017";
    const string databaseName = "local";

    MarketStackClient marketStackClient;
    FinancialDataStorageClient storageClient;

    public UnitTest1()
    {
        marketStackClient = new MarketStackClient(accessKey, new HttpClient());
        storageClient = new FinancialDataStorageClient(mongoDbConnectionString, databaseName);
    }

    private async Task Insert(int pageNumber, int offset, int accumulatedCount, int? total)
    {
        // In case we run in infinite loop
        // We limit the number of recursive query to 500.
        if (pageNumber == 500 || accumulatedCount == total)
        {
            return;
        }

        var response = await marketStackClient.GetEndOfDayAsync(offset, new List<string> { "DOW" });
        await storageClient.InsertEndOfDayItems(response.data);

        await Insert(
            pageNumber: pageNumber + 1,
            offset: response.pagination.offset + response.pagination.limit,
            accumulatedCount: accumulatedCount + response.data.Count(),
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
}