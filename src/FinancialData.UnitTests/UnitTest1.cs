using FinancialData.Storage;
using FinancialData.MarketStack;
using Xunit;

namespace FinancialData.UnitTests;

public class UnitTest1
{
    static string accessKey = "<Put here your Access Key>";
    static string mongoDbConnectionString = "<Put here your Connection string>";
    static string databaseName = "<Put here your Database name>";

    private static MarksetStackClient marketStackClient = new MarksetStackClient(accessKey);
    private static FinancialDataStorageClient storageClient = new FinancialDataStorageClient(mongoDbConnectionString, databaseName);

    public async Task Insert(int pageNumber, int offset, int accumulatedCount, int? total)
    {
        // In case we run in infinite loop
        // We limit number of recursive query to 500.
        if (pageNumber == 500 || (total != null && accumulatedCount == total))
        {
            return;
        }

        var response = await marketStackClient.GetEndOfDayDayAsync(offset, Symbols.US30);
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
}