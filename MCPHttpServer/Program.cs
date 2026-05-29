using MCPStdioServer.Tools;

namespace MCPHttpServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(
                        "http://localhost:6274", // TODO: remove from hardcoded values, move to settings.
                        "placeholder.com"
                    )
                    .WithMethods("GET", "POST")
                    .WithHeaders("Content-Type", "Accept");
            });
        });
        
        builder.Services.AddMcpServer()
            .WithHttpTransport(options => options.Stateless = true)
            .WithTools<FileSystemTools>();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthorization();
        app.UseCors();
        app.MapMcp("/mcp");
        app.MapGet("/", () => "MCP SSE Server is running.");

        app.Run();
    }
}