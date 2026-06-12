// This will create a new modpack instance to the user's Prism Launcher installation.
// 
// Modpack instance structure:
// - Modpack Name
//   - updater.exe (handles the modlist, config, and minecraft version updates)
//   - mmc-pack.json (the modpack instance's metadata, such as the modpack name, version, and author)
//   - manifest.json (the currently installed release version from GitHub, used for update checks)
//   - minecraft
//     - mods
//     - config
//     - resourcepacks
//     - shaderpacks
//     - icon.png (the modpack instance's icon, used for the instance's thumbnail in Prism Launcher)
//
// All other files and directories will be installed and maintained by Prism Launcher

// Installer workflow:
// 1. Create a UI using WPF for information and progress display
// 2. Check for the Prism Launcher installation and prompt the user to install it if not found.
// 3. Prompt the user to input the modpack name (Default is set).
// 4. Create the modpack instance directory and subdirectories.
// 5. Download the modpack's manifest.json from GitHub and save it to the modpack instance directory.
// 6. Create the mmc-pack.json file with the modpack instance's metadata and save it to the modpack instance directory.
// 7. Download the modpack's icon.png from GitHub and save it to the modpack instance directory.
// 8. Create the updater.exe file and save it to the modpack instance directory.
// 9. Create a shortcut to the updater.exe on the user's desktop for easy access to the modpack instance.

using System.Configuration;
using System.Data;
using System.Windows;

namespace installer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		this.ShutdownMode = ShutdownMode.OnMainWindowClose;

		// Check for Prism Launcher installation and inform the UI if missing
		var prismService = new Services.PrismLauncherService();
		var vm = new ViewModels.MainWindowViewModel();
		if (!prismService.IsInstalled(out var path))
		{
			vm.StatusMessage = "Prism Launcher not found. Please install Prism Launcher to continue.";
			vm.IsPrerequisiteMissing = true;
		}
		else
		{
			vm.StatusMessage = $"Prism Launcher found: {path}";
		}

		var mainWindow = new Views.MainWindow(vm);
		this.MainWindow = mainWindow;
		mainWindow.Show();
	}
}

