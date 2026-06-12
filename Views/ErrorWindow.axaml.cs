using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OddiloveVybaveni.Views;

public partial class ErrorWindow : Window
{
    public ErrorWindow() => InitializeComponent();

    public ErrorWindow(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
    }

    private void OnOkClick(object? sender, RoutedEventArgs e) => Close();
}
