using SmartDrawerWpfApp.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SmartDrawerWpfApp
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

        }
        private static void LogUnhandledException(Exception exception, string s)
        {
            const string DirectoryName = @"C:\Temp\SmartDrawerLog\";

            if (!Directory.Exists(DirectoryName))
            {
                Directory.CreateDirectory(DirectoryName);
            }

            var contents =
                string.Format(
                    "HResult:    {1}{0}" + "HelpLink:   {2}{0}" + "Message:    {3}{0}" + "Source:     {4}{0}"
                    + "StackTrace: {5}{0}" + "{0}",
                    Environment.NewLine,
                    exception.HResult,
                    exception.HelpLink,
                    exception.Message,
                    exception.Source,
                    exception.StackTrace);

            File.AppendAllText(string.Format("{0}Exceptions.log", DirectoryName), contents);

            Application.Current.Dispatcher.Invoke(() =>
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(exception, s);
                exp.ShowDialog();
            });
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {







            AppDomain.CurrentDomain.UnhandledException +=
              (s, exception) =>
              LogUnhandledException((Exception)exception.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException +=
                (s, exception) =>
                LogUnhandledException(exception.Exception, "Application.Current.DispatcherUnhandledException");

            TaskScheduler.UnobservedTaskException +=
                (s, exception) =>
                LogUnhandledException(exception.Exception, "TaskScheduler.UnobservedException");
        }
    }
}
