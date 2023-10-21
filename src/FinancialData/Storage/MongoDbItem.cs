namespace FinancialData.Storage;

using MongoDB.Bson;

public interface IMongoDbItem
{
    public ObjectId _id { get; set; }
}        