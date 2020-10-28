using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppSeriLog
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile(AppDomain.CurrentDomain.BaseDirectory + "\\appsettings.json", optional: false, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();

            var serilogLogger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

            Log.Logger = serilogLogger;

            Task.Run(() => { RepeatWriteLog(configuration); });


            Console.ReadLine();
        }

        private static Task RepeatWriteLog(IConfigurationRoot configuration)
        {
            string fullPath = configuration.GetSection("healthlog").GetValue<string>("fullPath");
            int intervalInSeconds = configuration.GetSection("healthlog").GetValue<int>("intervalInSeconds");
            int retainedFileCountLimit = configuration.GetSection("healthlog").GetValue<int>("retainedFileCountLimit");
            long fileSizeLimitBytes = configuration.GetSection("healthlog").GetValue<long>("fileSizeLimitBytes");

            var healthlog = new LoggerConfiguration()
                .WriteTo.File(fullPath, fileSizeLimitBytes: fileSizeLimitBytes, rollOnFileSizeLimit: true, retainedFileCountLimit: retainedFileCountLimit)
                .CreateLogger();

            while (true)
            {
                healthlog.Information($"{DateTime.Now}");
                Thread.Sleep(1000 * intervalInSeconds);
            }
        }
    }
}
