using System.Threading;
using System.Threading.Tasks;

namespace installer.Services;

public class InstallerService
{
    public InstallerService()
    {
    }

    public async Task InstallAsync(string instancePath, CancellationToken token)
    {
        // Orchestrate the install steps here (create folders, download, write files)
        await Task.CompletedTask;
    }
}
