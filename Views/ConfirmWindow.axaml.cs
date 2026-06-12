using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OddiloveVybaveni.Views;

public partial class ConfirmWindow : Window
{
    public ConfirmWindow() => InitializeComponent();

    public ConfirmWindow(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
    }

    private void OnYesClick(object? sender, RoutedEventArgs e) => Close(true);
    private void OnNoClick(object? sender, RoutedEventArgs e) => Close(false);
}
