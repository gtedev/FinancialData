using Quartz;

namespace FinancialData.Jobs;

public class GetFinancialDataJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Code that sends a periodic email to the user (for example)
        // Note: This method must always return a value 
        // This is especially important for trigger listers watching job execution 
        var httpClient = new HttpClient();
        var uri = new Uri("myUri");

        var response = await httpClient.GetStringAsync(uri);
        Console.WriteLine("Response" + response);
    }
}