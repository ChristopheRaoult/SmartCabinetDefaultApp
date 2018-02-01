using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddHTTP
{
    class Program
    {
        static void Main(string[] args)
        {
            // Add port to http 
            Console.WriteLine("\r\n-------------- Binding port 9004 to Local user: -------------- ");
            Console.WriteLine("\r\nTry to delete reservation if exists: ");
            RunCommandLine("netsh", "http delete urlacl url=http://+:9004/SslWallNotificationService");
            Console.WriteLine("\r\nTry to add reservation : ");
            RunCommandLine("netsh", "http add urlacl url=http://+:9004/SslWallNotificationService sddl=D:(A;;GX;;;S-1-1-0)");


            //Add Firewall rule 
            Console.WriteLine("-------------- Add firewall rule for  port 9004: -------------- ");
            RunCommandLine("netsh", "advfirewall firewall add rule name = \"Open Port 9004 for WallApp\" dir =in action = allow protocol = TCP localport = 9004");
            Console.ReadLine();
        }

        private static void RunCommandLine(string filePath, string arguments)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(filePath, arguments);
            p.StartInfo.Verb = "runas";
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit(10000);
            System.Threading.Thread.Sleep(1000);
        }
    }
}
