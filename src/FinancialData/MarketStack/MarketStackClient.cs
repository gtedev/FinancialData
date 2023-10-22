namespace FinancialData.MarketStack;

using JsonSerializer = System.Text.Json.JsonSerializer;

public class MarketStackClient
{
    private int limit = 1000;
    private HttpClient httpClient;
    private string endOfDayUrl;

    public MarketStackClient(string accessKey, HttpClient httpClient)
    {
        endOfDayUrl = $"http://api.marketstack.com/v1/eod?access_key={accessKey}";
        this.httpClient = httpClient;
    }

    public async Task<EndOfDayResponse> GetEndOfDayAsync(int offset, IEnumerable<string> symbols)
    {
        var uri = new Uri($"{endOfDayUrl}&limit={limit}&offset={offset}&symbols={string.Join(",", symbols)}");

        var responseString = await httpClient.GetStringAsync(uri);
        var response = JsonSerializer.Deserialize<EndOfDayResponse>(responseString);

        return response;
    }
}