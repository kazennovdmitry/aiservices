using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace CoreLogic.Plugins;

public class FileSystemPlugin
{
    [KernelFunction]
    [Description("Get free disk space information")]
    public string GetDiskSpaceInfo()
    {
        var report = new List<string>();
        foreach (var drive in DriveInfo.GetDrives())
        {
            try
            {
                if (drive.IsReady)
                {
                    var available = (drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0)).ToString("F2");
                    var total = ((double)drive.TotalSize / (1024.0 * 1024.0 * 1024.0)).ToString("F2");
                    report.Add($"{drive.Name} ({drive.DriveFormat}): Available {available} GB from {total} GB");
                }
            }
            catch
            {
                report.Add($"{drive.Name}: Access denied or unavailable");
            }
        }

        return string.Join("\n", report) ?? "No drives available.";
    }
}
