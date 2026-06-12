using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using installer.Helpers;

namespace installer.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private string _title = "Modpack Installer";
    private string _statusMessage = string.Empty;
    private bool _prerequisiteMissing = false;
    private string _modpackName = string.Empty;

    // Properties bound to the UI

    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public bool IsPrerequisiteMissing
    {
        get => _prerequisiteMissing;
        set { _prerequisiteMissing = value; OnPropertyChanged(); }
    }

    public string ModpackName
    {
        get => _modpackName;
        set { _modpackName = value; OnPropertyChanged(); ((RelayCommand)InstallCommand).RaiseCanExecuteChanged(); }
    }

    // INotifyPropertyChanged implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Commands

    public ICommand InstallCommand { get; }

    public MainWindowViewModel()
    {
        InstallCommand = new RelayCommand(StartInstall, CanInstall);
    }

    private void StartInstall()
    {
        // Placeholder for installation logic
        StatusMessage = "Starting installation...";
    }

    private bool CanInstall()
    {
        // Installation can only start if prerequisites are met AND modpack name is valid
        return !IsPrerequisiteMissing && !string.IsNullOrWhiteSpace(ModpackName);
    }
}
