
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.AI;
using OllamaChat.Factories;
using Qdrant.Client;
namespace OllamaChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();
            //builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2"));
            builder.Services.AddSingleton(new QdrantClient("localhost", 6334));
            //builder.Services.AddSingleton(new QdrantClient(
            //        new QdrantClientOptions
            //        {
            //            Url = "http://localhost:6333" // Qdrant server URL
            //        }));
            builder.Services.AddScoped<IVectorSearchFactory, VectorSearchFactory>();
            builder.Services.AddScoped<IChatFactory, ChatFactory>();

            //builder.Services.AddSwaggerGen(options =>
            //{
            //    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            //});
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseRouting();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            //var chatClient = app.Services.GetRequiredService<IChatClient>();

            //var chatCompletion =  chatClient.GetStreamingResponseAsync("What is .NET? Reply in 50 words max.");

            //Console.WriteLine(chatCompletion);
            app.Run();
        }
    }
}
