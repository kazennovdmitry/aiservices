using CoreLogic.Plugins;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace MCPServer.Tools;

[McpServerToolType]
public sealed class FileSystemTools
{
    private readonly ILogger<FileSystemTools> _logger;
    
    public FileSystemTools(ILogger<FileSystemTools> logger)
    {
        _logger = logger;
    }
    
    [McpServerTool, Description("Get free disk space information")]
    public async Task<string> GetDiskSpaceInfo([Description("Optional drive name filter (e.g., '/' or '/media')")] string driveName = "",
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Requesting disk space info for drive: {DriveName}", 
            string.IsNullOrEmpty(driveName) ? "All" : driveName);

        try
        {
            var plugin = new FileSystemPlugin();
            var result = await Task.Run(() => plugin.GetDiskSpaceInfo(driveName), cancellationToken);
            _logger.LogDebug("Disk space info retrieved successfully.");

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Disk space request was cancelled by the client.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting disk space info.");
            throw;
        }
    }
}
