namespace FinancialData.MarketStack;

using JsonSerializer = System.Text.Json.JsonSerializer;

public class MarksetStackClient
{
    private int limit = 1000;
    private HttpClient httpClient;
    private  string endOfDayUrl;

    public MarksetStackClient(string accessKey)
    {
        this.endOfDayUrl = $"http://api.marketstack.com/v1/eod?access_key={accessKey}&limit={limit}";
        this.httpClient = new HttpClient();
    }

    private Uri GetUrlWithOffset(int offset, IEnumerable<string> symbols)
    {
        return new Uri($"{endOfDayUrl}&offset={offset}&symbols={string.Join(",", symbols)}");
    }
    
    public async Task<EndOfDayResponse> GetEndOfDayDayAsync(int offset, IEnumerable<string> symbols)
    {
        var uri = GetUrlWithOffset(offset, symbols);
        var responseString = await this.httpClient.GetStringAsync (uri);
        var response = JsonSerializer.Deserialize<EndOfDayResponse>(responseString);

        return response;
    }
}