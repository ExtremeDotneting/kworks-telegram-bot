using CefSharp;
using RollingOutTools.Storage;
using RollingOutTools.Storage.JsonFileStorage;
using RollingOutTools.Storage.WithLiteDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace KworkTelegramBot
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsLaunchedWithSystem { get; private set; }


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IsLaunchedWithSystem=e.Args.Contains("/winstart");
        }

        public App()
        {
            Assembly curAssembly = Assembly.GetExecutingAssembly();
            Environment.CurrentDirectory = Path.GetDirectoryName(curAssembly.Location);

            var cacheDir = Path.Combine(Environment.CurrentDirectory, "webview_cahce");
            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = cacheDir,
                
            };

            settings.DisableTouchpadAndWheelScrollLatching();

            //Example of setting a command line argument
            //Enables WebRTC
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            //StorageHardDrive.InitDependencies(
            //    new LiteDatabaseLocalStorage()
            //    );

            StorageHardDrive.InitDependencies(
                new JsonLocalStorage()
                );

            
        }
    }
}
