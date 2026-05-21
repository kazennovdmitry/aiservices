using CoreLogic.Plugins;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

[McpServerToolType]
public sealed class FileSystemTools
{
    [McpServerTool, Description("Get free disk space information")]
    public string GetDiskSpaceInfo([Description("Optional drive name filter (e.g., '/' or '/media')")] string driveName = "")
    {
        var plugin = new FileSystemPlugin();
        return plugin.GetDiskSpaceInfo(driveName);
    }
}
