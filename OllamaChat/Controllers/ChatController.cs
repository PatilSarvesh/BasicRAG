using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OllamaChat.Factories;

namespace OllamaChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ChatController : ControllerBase
    {
        private readonly IVectorSearchFactory _vectorSearchFactory;

        private IChatClient _chatClient = new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2");

        //public ChatController(IChatClient chatClient)
        //{
        //    this.chatClient = chatClient;
        //}


        public ChatController(IVectorSearchFactory vectorSearchFactory)
        {
            _vectorSearchFactory = vectorSearchFactory;
           
        }

        [HttpGet(Name = "Chat")]
        public async Task<string> Get(string prompt)
        {
            var response = "";
            // Start the conversation with context for the AI model
            List<ChatMessage> chatHistory = new()
            {
                new ChatMessage(ChatRole.System, """
                    You are an experienced .NET Architect with over 15 years of expertise in designing and building enterprise-grade software solutions using the .NET ecosystem.
                    Your role is to provide clear, actionable advice on best practices, architecture patterns, performance optimization, security, and scalability for .NET applications.
                    When assisting someone, you always follow these steps:
                    1. Understand the context: Ask clarifying questions about the project requirements, team size, technology stack, and business goals.
                    2. Analyze the problem: Identify potential challenges, risks, and opportunities for improvement in the current design or approach.
                    3. Provide recommendations: Offer specific guidance on architecture decisions, tools, frameworks, and patterns that align with modern .NET development practices.
                    4. Include examples: Share code snippets, diagrams, or references to help illustrate your recommendations.
                    5. Summarize key points: Recap the main takeaways and ask if there is anything else you can assist with.

                    Key areas of expertise:
                    - Microservices and distributed systems using .NET Core/ASP.NET Core
                    - Clean Architecture and Domain-Driven Design (DDD)
                    - Cloud-native development with Azure (e.g., Azure Functions, Kubernetes, Service Bus)
                    - Performance tuning and memory management in .NET
                    - Security best practices (e.g., authentication, authorization, data protection)
                    - DevOps and CI/CD pipelines for .NET applications
                    - Database design and integration (SQL Server, Cosmos DB, Entity Framework Core)

                    Always introduce yourself when first saying hello, and ensure your responses are professional, concise, and tailored to the user's needs.
                """)
            };

            while (true)
            {
                // Get user prompt and add to chat history
                var userPrompt = prompt;
                chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

                // Stream the AI response and add to chat history
                await foreach (var item in
                    _chatClient.GetStreamingResponseAsync(chatHistory))
                {
                    Console.Write(item.Text);
                    response += item.Text;
                }
                chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
                return response;
            }

           
        }
        [HttpPost("abc/CreateCollection")]
        public async Task<string> CreateCollection(string collectionName)
        {
            return await _vectorSearchFactory.CreateCollection(collectionName);
        }
        [HttpPost("abc/Uploaddata")]
        public async Task UpsertDataAsync([FromBody] UpsertRequest request)
        {
            await _vectorSearchFactory.UpsertDataAsync(request.CollectionName, request.Messages);
        }

        [HttpPost("abc/search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var results = await _vectorSearchFactory.SearchData(
                collectionName: request.CollectionName,
                query: request.Query,
                limit: request.Limit
            );
            return Ok(results);
        }
    }
    public class UpsertRequest
    {
        public string CollectionName { get; set; }
        public List<string> Messages { get; set; } // Use List<string> instead of IEnumerable<string>
    }
    public class SearchRequest
    {
        public string CollectionName { get; set; }
        public string Query { get; set; }
        public int Limit { get; set; } = 3; // Default limit
    }
}
