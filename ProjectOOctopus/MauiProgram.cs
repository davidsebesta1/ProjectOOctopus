using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Mopups.Hosting;
using ProjectOOctopus.Pages;
using ProjectOOctopus.Services;
using ProjectOOctopus.ViewModels;

namespace ProjectOOctopus
{
    /// <summary>
    /// Starting class of the Maui application
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Static application instance singleton
        /// </summary>
        public static MauiApp AppInstance { get; private set; }

        /// <summary>
        /// Creator method with decorator pattern
        /// </summary>
        /// <returns></returns>
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
                }).ConfigureLifecycleEvents(events =>
                {

#if WINDOWS
                    events.AddWindows(windowsLifecycleBuilder =>
                    {
                        windowsLifecycleBuilder.OnWindowCreated(window =>
                        {
                            if (window.Title == "ProjectOOctopus")
                            {
                                var windows = window;
                                nint handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                                WindowId id = Win32Interop.GetWindowIdFromWindow(handle);
                                AppWindow appWindow = AppWindow.GetFromWindowId(id);

                                appWindow.Closing += async (s, e) =>
                                {
                                    e.Cancel = true;

                                    foreach (Window win in Application.Current.Windows)
                                    {
                                        if (win.Title == null)
                                        {
                                            bool result = await Shell.Current.DisplayAlert("Exit Confirmation", "You sure want to close app? Unsaved progress will be lost!", "Close App", "Cancel");

                                            if (result)
                                            {
                                                Application.Current.CloseWindow(win);
                                            }
                                        }
                                    }

                                };
                            }

                        });
                    });
                });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<RoleManagerViewModel>();

            builder.Services.AddSingleton<EmployeesService>();
            builder.Services.AddSingleton<ProjectsService>();
            builder.Services.AddSingleton<RolesService>();
            builder.Services.AddSingleton<CsvLoader>();
            builder.Services.AddSingleton<ExcelExporterService>();
            builder.Services.AddSingleton<ExcelImporterService>();

            builder.Services.AddTransient<EntryValidatorService>();

            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<RoleManagerPopup>();

            AppInstance = builder.Build();

            ServicesHelper.Init(AppInstance.Services);

            return AppInstance;
        }
    }
}
