using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.StaticHelpers
{
    public static class Utils
    {
        public static string GetLocalIp()
        {

            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

    }

    public  class PerfTimerLogger : IDisposable
    {
        public PerfTimerLogger(string message)
        {
            this._message = message;
            this._timer = new Stopwatch();
            this._timer.Start();
        }

        string _message;
        Stopwatch _timer;

        public void Dispose()
        {
            this._timer.Stop();
            var ms = this._timer.ElapsedMilliseconds;

            LogToFile.LogMessageToFile(string.Format("{0} - Elapsed Milliseconds: {1}", this._message, ms));

            // log the performance timing with the Logging library of your choice
            // Example:
            // Logger.Write(
            //     string.Format("{0} - Elapsed Milliseconds: {1}", this._message, ms)
            // );
        }
    }

    public static class LogToFile
    {
        public static string GetTempPath()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\";
            return path;
        }

        static object lockMethod = new object();
        public static void LogMessageToFile(string msg)
        {
            lock (lockMethod)
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(
                GetTempPath() + "smartDrawerLog.txt");
                try
                {
                    string logLine = System.String.Format(
                        "{0:G}: {1}.", System.DateTime.Now, msg);
                    sw.WriteLine(logLine);
                }
                finally
                {
                    sw.Close();
                }
            }
        }
    }
}
