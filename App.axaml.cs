// ============================================================
// TŘÍDA APP — "JÁDRO" AVALONIA APLIKACE
// Spouští se ihned po Program.cs
// Zodpovídá za: inicializaci DI, vytvoření hlavního okna
// ============================================================

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OddiloveVybaveni.ViewModels;
using OddiloveVybaveni.Views;

namespace OddiloveVybaveni;

public class App : Application
{
    // Zavolá se jako první — načte App.axaml (DataTemplates, styly, témata)
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    // Zavolá se když je Avalonia framework plně připraven
    public override void OnFrameworkInitializationCompleted()
    {
        // Inicializuje DI kontejner (zaregistruje všechny služby, repositories, ViewModely)
        AppServices.Initialize();

        // IClassicDesktopStyleApplicationLifetime = desktop aplikace (ne mobilní/webová)
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                // Z DI kontejneru dostaneme MainWindowViewModel (je zaregistrován jako Singleton)
                // DataContext = "datový zdroj" okna — View se na něj napojí přes Binding
                DataContext = AppServices.Provider.GetRequiredService<MainWindowViewModel>()
            };
        }

        // Důležité: zavolat základ, jinak Avalonia neskončí správně
        base.OnFrameworkInitializationCompleted();
    }
}
