using System.ServiceProcess;

namespace IceLockWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (OperatingSystem.IsWindows() && !Environment.UserInteractive)
            {
                // Run as a Windows Service using ServiceBase
                ServiceBase.Run(new IFreezeService());
            }
            else
            {
                // Run as a console app for debugging or interactive testing
                var builder = Host.CreateApplicationBuilder(args);
                builder.Services.AddHostedService<Worker>(); // Optional: Your original Worker Service logic

                var host = builder.Build();
                host.Run();
            }
           
        }
    }
}