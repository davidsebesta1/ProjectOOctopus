using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using ProjectOOctopus.Services;
using ProjectOOctopus.ViewModels;


namespace ProjectOOctopus
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddSingleton<EmployeesService>();
            builder.Services.AddSingleton<ProjectsService>();

            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
