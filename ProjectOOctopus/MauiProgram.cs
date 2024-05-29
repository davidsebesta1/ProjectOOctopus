﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using ProjectOOctopus.Pages;
using ProjectOOctopus.Services;
using ProjectOOctopus.ViewModels;

namespace ProjectOOctopus
{
    public static class MauiProgram
    {
        public static MauiApp AppInstance { get; private set; }

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
            builder.Services.AddSingleton<RoleManagerViewModel>();

            builder.Services.AddSingleton<EmployeesService>();
            builder.Services.AddSingleton<ProjectsService>();
            builder.Services.AddSingleton<RolesService>();
            builder.Services.AddSingleton<CsvLoader>();
            builder.Services.AddSingleton<ExcelService>();

            builder.Services.AddTransient<EntryValidatorService>();

            builder.Services.AddTransient<MainPage>();

            builder.Services.AddTransient<RoleManagerPopup>();

            AppInstance = builder.Build();

            ServicesHelper.Init(AppInstance.Services);

            return AppInstance;
        }
    }
}
