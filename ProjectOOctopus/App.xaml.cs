﻿
using Microsoft.Extensions.Logging;

namespace ProjectOOctopus
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

        }
    }
}
