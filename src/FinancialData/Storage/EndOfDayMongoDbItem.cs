namespace FinancialData.Storage;

using MarketStack;
using MongoDB.Bson;

public class EndOfDayMongoDbItem : EndOfDayItem, IMongoDbItem
{
    public ObjectId _id { get; set; }
}