// ============================================================
// DEPENDENCY INJECTION KONTEJNER
// Centrální místo kde se registrují všechny závislosti aplikace.
// Každá třída zde říká: "když někdo potřebuje X, dej mu Y"
// ============================================================

using Microsoft.Extensions.DependencyInjection;
using OddiloveVybaveni.Repositories;
using OddiloveVybaveni.Services;
using OddiloveVybaveni.ViewModels;

namespace OddiloveVybaveni;

public static class AppServices
{
    // Privátní reference na kontejner — přístupná jen přes property Provider
    private static ServiceProvider? _provider;

    // Veřejný přístup ke kontejneru
    // ?? throw = pokud by někdo zavolal Provider před Initialize(), dostane čitelnou chybu
    public static ServiceProvider Provider =>
        _provider ?? throw new InvalidOperationException("AppServices not initialized.");

    public static void Initialize()
    {
        // Načte DATABASE_URL z prostředí (.env souboru)
        // ?? throw = pokud proměnná chybí, aplikace okamžitě spadne s vysvětlující chybou
        var connStr = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? throw new InvalidOperationException(
                "Proměnná DATABASE_URL není nastavena. Zkontroluj soubor .env");

        // ServiceCollection = "seznam" závislostí, které chceme zaregistrovat
        var services = new ServiceCollection();

        // --- INFRASTRUKTURA ---

        // AddSingleton = existuje JEDINÁ instance po celou dobu běhu aplikace
        // DatabaseConfig jen obaluje connection string — sdílíme ho napříč celou app
        services.AddSingleton(new DatabaseConfig(connStr));

        // --- REPOSITORIES (přístup k databázi) ---

        // AddTransient = NOVÁ instance při každém použití
        // Repositories jsou bezstavové, takže nám to vyhovuje
        // IKategorieRepository je interface — DI kontejner ho naplní třídou KategorieRepository
        services.AddTransient<IKategorieRepository, KategorieRepository>();
        services.AddTransient<IVybaveniRepository, VybaveniRepository>();
        services.AddTransient<IVypujckaRepository, VypujckaRepository>();

        // --- SERVICES ---

        // DialogService zajišťuje otevírání modálních oken (formuláře, chybové dialogy)
        services.AddTransient<IDialogService, DialogService>();

        // --- VIEWMODELS ---

        // Transient = nová instance při každé navigaci (čistý stav)
        services.AddTransient<VybaveniListViewModel>();
        services.AddTransient<VybaveniDetailViewModel>();

        // Singleton = hlavní ViewModel existuje po celou dobu — řídí navigaci celého okna
        services.AddSingleton<MainWindowViewModel>();

        // Sestaví a uzamkne kontejner — od teď lze volat Provider.GetRequiredService<T>()
        _provider = services.BuildServiceProvider();
    }
}
