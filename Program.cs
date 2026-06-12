// ============================================================
// VSTUPNÍ BOD APLIKACE
// Toto je první kód, který se spustí po spuštění .exe
// ============================================================

using Avalonia;
using OddiloveVybaveni;

// 1. Načte soubor .env ze složky projektu (nebo nadřazené složky)
//    .env obsahuje DB_HOST, DB_PORT, DB_USER, DB_PASSWORD, DB_NAME
//    TraversePath() = hledá .env soubor od aktuální složky směrem nahoru
DotNetEnv.Env.TraversePath().Load();

// 2. Spustí Avalonia aplikaci
AppBuilder.Configure<App>()       // říká Avalonii: použij naši třídu App jako základ
    .UsePlatformDetect()          // automaticky detekuje OS (Windows / Linux / macOS)
    .WithInterFont()              // načte Inter písmo (zabudované v balíčku Avalonia)
    .LogToTrace()                 // zapisuje debug logy do Trace výstupu (viditelné v IDE)
    .StartWithClassicDesktopLifetime(args); // spustí jako klasická desktopová aplikace
