using Microsoft.Extensions.DependencyInjection;
using OddiloveVybaveni.Repositories;
using OddiloveVybaveni.Services;
using OddiloveVybaveni.ViewModels;

namespace OddiloveVybaveni;

public static class AppServices
{
    private static ServiceProvider? _provider;

    public static ServiceProvider Provider =>
        _provider ?? throw new InvalidOperationException("AppServices not initialized.");

    public static void Initialize()
    {
        var connStr = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? throw new InvalidOperationException(
                "Proměnná DATABASE_URL není nastavena. Zkontroluj soubor .env");

        var services = new ServiceCollection();

        services.AddSingleton(new DatabaseConfig(connStr));
        services.AddTransient<IKategorieRepository, KategorieRepository>();
        services.AddTransient<IVybaveniRepository, VybaveniRepository>();
        services.AddTransient<IVypujckaRepository, VypujckaRepository>();
        services.AddTransient<IDialogService, DialogService>();
        services.AddTransient<VybaveniListViewModel>();
        services.AddTransient<VybaveniDetailViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        _provider = services.BuildServiceProvider();
    }
}
