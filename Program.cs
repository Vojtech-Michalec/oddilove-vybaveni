using Avalonia;
using OddiloveVybaveni;

DotNetEnv.Env.TraversePath().Load();
AppBuilder.Configure<App>()
    .UsePlatformDetect()
    .WithInterFont()
    .LogToTrace()
    .StartWithClassicDesktopLifetime(args);
