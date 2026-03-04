
using Infrastructure.DataContext;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace IceLockWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private const string PipeName = "IceLockWorkerPipe";
        // Store variables in the Worker Service
        private int _storedNumber = 0;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await StartNamedPipeServer(stoppingToken);
            }
        }


        private async Task StartNamedPipeServer(CancellationToken stoppingToken)
        {
            // Create the named pipe without security settings.
            using var pipeServer = new NamedPipeServerStream(
                PipeName,
                PipeDirection.InOut,
                1,  // Maximum number of server instances
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous,
                0,  // InBufferSize
                0   // OutBufferSize
            );

            //// Create a PipeSecurity object to define access rules.
            //var pipeSecurity = new PipeSecurity();

            //// Add access rule for the "Users" group with full read/write access.
            //pipeSecurity.AddAccessRule(new PipeAccessRule(
            //    new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), // Grants access to the "Users" group
            //    PipeAccessRights.ReadWrite, // Allow reading and writing
            //    AccessControlType.Allow));

            //// Apply the security settings to the pipe.
            //pipeServer.SetAccessControl(pipeSecurity);

            
            // Wait for a client connection.
            await pipeServer.WaitForConnectionAsync(stoppingToken);

            using var reader = new StreamReader(pipeServer);
            using var writer = new StreamWriter(pipeServer) { AutoFlush = true };

            string received = await reader.ReadLineAsync();
            if (int.TryParse(received, out int command))
            {
                _logger.LogInformation("Received command: {command}", command);

                // handle client-side data needed
                string response = HandleCommand(command);

                // send response to WPF
                await writer.WriteLineAsync(response);
            }
            else
            {
                _logger.LogWarning("Invalid command received.");

                // send response Invalid command to WPF
                await writer.WriteLineAsync("Invalid command.");
            }

            // Disconnect the pipe to allow for the next client connection.
            //pipeServer.Disconnect();
            
        }



        private string HandleCommand(int command)
        {
            switch (command)
            {
                case 1:
                    //_storedNumber++;
                    //return $"Incremented stored number: {_storedNumber}";
                    AppConfigAndLoginContext context = new AppConfigAndLoginContext();

                    return context.Application_Configuration.FirstOrDefault().DeviceSN.ToString();

                case 2:
                    _storedNumber--;
                    return $"Decremented stored number: {_storedNumber}";

                case 3:
                    return $"Current stored number: {_storedNumber}";

                default:
                    return "Unknown command.";
            }
        }


    }
}

