namespace FinancialData.MarketStack;

public class Pagination
{
    public int limit { get; set; }
    public int offset { get; set; }

    public int count { get; set; }
    public int total { get; set; }
}

public class MarketStackResponse<TData>
{
    public Pagination pagination { get; set; }
    public List<TData> data { get; set; }
}