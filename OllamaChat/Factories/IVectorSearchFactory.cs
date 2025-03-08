

namespace OllamaChat.Factories
{
    public interface IVectorSearchFactory
    {
        Task<string> CreateCollection(string collectionName);
        Task<string> SearchData(string collectionName, string query, int limit = 3);
        Task UpsertDataAsync(string collectionName, IEnumerable<string> messages);
    }
}