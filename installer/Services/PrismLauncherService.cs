using System;
using System.IO;

namespace installer.Services;

/// <summary>
/// Locates a Prism Launcher installation using common paths.
/// </summary>
public class PrismLauncherService
{
    public bool IsInstalled(out string? installPath)
    {
        // Check some common locations for Prism Launcher on Windows
        var candidates = new[] {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PrismLauncher"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "prism-launcher"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrismLauncher"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "prism-launcher"),
        };

        foreach (var p in candidates)
        {
            if (string.IsNullOrEmpty(p))
                continue;

            if (Directory.Exists(p))
            {
                installPath = p;
                return true;
            }
        }

        installPath = null;
        return false;
    }
}
