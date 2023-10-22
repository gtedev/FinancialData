namespace FinancialData.Storage;

using MongoDB.Bson;

public static class Extensions
{
    public static BsonValue Field(this BsonDocument document, string name)
    {
        var element = document
            .Elements
            .FirstOrDefault(r => r.Name == name);

        return element.Value;
    }
}