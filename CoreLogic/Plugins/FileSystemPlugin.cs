using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace CoreLogic.Plugins;

public class FileSystemPlugin
{
    [KernelFunction]
    [Description("Get free disk space information")]
    public string GetDiskSpaceInfo([Description("Optional name of the drive or mount point to filter by (e.g., '/'). If empty, returns all real drives.")] 
        string driveName = "")
    {
        var targetDrive = driveName?.Trim();
        var report = new List<string>();
        foreach (var drive in DriveInfo.GetDrives())
        {
            try
            {
                if (!string.IsNullOrEmpty(targetDrive))
                {
                    bool isMatch = drive.Name.Equals(targetDrive, StringComparison.OrdinalIgnoreCase) ||
                                   drive.Name.TrimEnd('/').Equals(targetDrive.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);
            
                    if (!isMatch) continue;
                }
                
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
