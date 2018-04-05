using MahApps.Metro;
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
            try
            {
                if (MultipleInstance()) // Check first multiple instance of application
                {
                    MessageBox.Show("More than one instance is running");
                    System.Threading.Thread.Sleep(1000);
                    ProcessKiller(); // Kill the current process
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

 
    /// <summary>
    /// Check Multiple Instance
    /// </summary>
    /// <returns>Boolean</returns>
    private static Boolean MultipleInstance()
    {
        Boolean Flag = false;
        try
        {
            System.Diagnostics.Process[] ProcessObj = null; ;

            // Name of Process module
            String ModualName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName.ToString();

            // Get a process name
            String ProcessName = System.IO.Path.GetFileNameWithoutExtension(ModualName);

            // Get all instances of Current application running on the local computer.

            ProcessObj = System.Diagnostics.Process.GetProcessesByName(ProcessName);

            if (ProcessObj.Length > 1) // if multipal application is running then it is true otherwise it is false
            {
                Flag = true;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return Flag;
    }
    private static void ProcessKiller()
    {

        try
        {
            System.Diagnostics.Process ProcessObj = System.Diagnostics.Process.GetCurrentProcess();
            ProcessObj.Kill(); // kill the current process
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

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
