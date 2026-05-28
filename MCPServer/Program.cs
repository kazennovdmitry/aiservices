using MCPServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MCPServer;

class Program
{
    static async Task<int> Main(string[] args)
    {
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
