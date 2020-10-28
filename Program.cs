using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleAppSeriLog
{
    class Program
    {
        static IConfigurationRoot _configuration;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            _configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile(AppDomain.CurrentDomain.BaseDirectory + "\\appsettings.json", optional: false, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();

            int intervalInSeconds = _configuration.GetSection("healthlog").GetValue<int>("intervalInSeconds");

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = intervalInSeconds * 1000;
            aTimer.Enabled = true;

            Console.ReadLine();
        }
        // Specify what you want to happen when the Elapsed event is raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            string fullPath = _configuration.GetSection("healthlog").GetValue<string>("fullPath");
            using (TextWriter tw = new StreamWriter(path: fullPath, append: false))
            {
                tw.WriteLine(DateTime.Now);
            }
        }
    }
}
