using System.Windows;
using installer.ViewModels;

namespace installer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
    }

    public MainWindow(MainWindowViewModel vm)
    {
        InitializeComponent();
        this.DataContext = vm ?? new MainWindowViewModel();
    }
}
