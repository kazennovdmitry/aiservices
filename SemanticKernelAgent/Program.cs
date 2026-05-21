using CoreLogic.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelAgent;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var openaiConfig = builder.Configuration.GetSection("OpenAI");

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        builder.Services.AddOpenAIChatCompletion(
            modelId: openaiConfig["ModelId"] ?? "local-llama",
            apiKey: "placeholder",
            endpoint: new Uri(openaiConfig["Endpoint"] ?? "http://localhost:8080/v1")
        );

        builder.Services.AddTransient(sp =>
        {
            var kernel = new Kernel(sp);
            kernel.Plugins.AddFromType<FileSystemPlugin>("FileUtils");
            return kernel;
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthorization();

        app.MapPost("/api/agent/chat", async (string userPrompt, Kernel kernel, ILogger<Program> logger) =>
        {
            try
            {
                var chatService = kernel.GetRequiredService<IChatCompletionService>();

                OpenAIPromptExecutionSettings settings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

                var history = new ChatHistory();
                history.AddUserMessage(userPrompt);

                var response = await chatService.GetChatMessageContentAsync(history, settings, kernel);

                return Results.Ok(new { Answer = response.Content });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[AI Agent Error]: {Message}", ex.Message);
                return Results.Json(new { error = ex.Message, details = ex.InnerException?.Message }, statusCode: 500);
            }
        });

        app.Run();
    }
}