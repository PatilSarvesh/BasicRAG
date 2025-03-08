using Qdrant.Client;
using Qdrant.Client.Grpc;


namespace OllamaChat.Factories
{
    public class VectorSearchFactory : IVectorSearchFactory
    {
      
        private readonly string _qdrantUrl = "http://localhost:6333";
        private readonly string _collectionName = "chat";
        private readonly QdrantClient _client;
        public VectorSearchFactory(QdrantClient qdrant)
        {
            _client = qdrant;
        }

        public async Task<string> CreateCollection(string collectionName)
        {
            //var response = await _httpClient.PostAsync($"{_qdrantUrl}/collections", new StringContent(JsonConvert.SerializeObject(new { name = collectionName }), Encoding.UTF8, "application/json"));
            //return await response.Content.ReadAsStringAsync();


            await _client.CreateCollectionAsync(collectionName: collectionName, vectorsConfig: new VectorParams
            {
                Size = 4,
                Distance = Distance.Cosine
            });
            return "Collection created";
        }


        // New Upsert Method
        public async Task UpsertDataAsync(string collectionName, IEnumerable<string> messages)
        {
            // Generate points (vectors + metadata)
            var points = messages.Select(message => new PointStruct
            {
                Id = new PointId { Uuid = Guid.NewGuid().ToString() }, // Unique ID
                Vectors = GenerateEmbedding(message) , // Vector
                Payload = 
                        {
                            { "message", message }, // Your message
                            { "category", "chat" }, // Custom metadata
                            
                        }
            }).ToList();

            // Upsert into Qdrant
            await _client.UpsertAsync(
                collectionName: collectionName,
                points: points
            );
        }

        public async Task<string> SearchData(string collectionName, string query, int limit = 3)
        {
            float[] queryVector = GenerateEmbedding(query);
            var searchResult = await _client.QueryAsync(
                collectionName: collectionName,
                query: queryVector,
                limit: ((ulong)limit),
                payloadSelector: true
                );
            //return searchResult.Points.Select(point => new SearchResult
            //{
            //    Message = point.Payload["message"]?.ToString(),
            //    Score = point.Score
            //}).ToList();
            return searchResult.ToString();
            

        }
        // Mock embedding generation (replace with a real model)
        private float[] GenerateEmbedding(string text)
        {
            // Example: Generate a 4-dimensional vector (matches collection config)
            var random = new Random();
            return new float[4] {
            (float)random.NextDouble(),
            (float)random.NextDouble(),
            (float)random.NextDouble(),
            (float)random.NextDouble()
        };
        }
    }
    public class SearchResult
    {
        public string Message { get; set; }
        public float Score { get; set; }
    }
}
