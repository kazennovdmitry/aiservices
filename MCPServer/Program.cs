using MCPServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MCPServer;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // This will prevent third-party libraries from dumping console output into stdout used for the MCP protocol.
        // But MCP SDK StdioServerTransport uses Console.OpenStandardInput() directly and is not affected.
        // https://github.com/modelcontextprotocol/csharp-sdk/blob/69a7fd39/src/ModelContextProtocol.Core/Server/StdioServerTransport.cs#L28-L34
        Console.SetOut(new StreamWriter(Console.OpenStandardError())
        {
            AutoFlush = true
        });
        
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools<FileSystemTools>();

        builder.Logging.AddConsole(options =>
        {
            options.LogToStandardErrorThreshold = LogLevel.Trace;
        });
        builder.Logging.SetMinimumLevel(LogLevel.Trace);

        var app = builder.Build();
        await app.RunAsync();
        return 0;
    }
}
