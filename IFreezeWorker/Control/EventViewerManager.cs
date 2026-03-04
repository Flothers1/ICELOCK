
using Infrastructure;
using System.Diagnostics;


namespace IceLockWorker
{
    class EventViewerManager
    {
        private const string ViewerProcessName = "Event_Viewer";
        private const string ViewerExecutablePath = @"C:\Users\Public\Ice Lock\Event_Viewer.exe";

    //    private static readonly string ViewerExecutablePath =
    //Path.Combine(MainProjectPath.ProjectPath, "Event_Viewer.exe");

        public void EnableEventViewer()
        {
          //  IFreezeService.Log("EnableEventViewer invoked");

            ThreadStart thstart = new ThreadStart(async () =>
            {
                try
                {
                 //   await IFreezeService.Log("Checking if Event Viewer is running...");
                    Process[] pname = Process.GetProcessesByName(ViewerProcessName);

                 //   await IFreezeService.Log($"Found {pname.Length} instances of {ViewerProcessName}");

                    if (pname.Length == 0)
                    {
                      //  await IFreezeService.Log("Event Viewer not running, attempting to start it...");
                        ExeRunner runner = new ExeRunner(ViewerExecutablePath);
                        runner.runprocess();
                      //  await IFreezeService.Log("Event Viewer has been started.");
                    }
                    else
                    {
                       // await IFreezeService.Log("Event Viewer is already running.");
                    }
                }
                catch (Exception ex)
                {
                    await IFreezeService.Log("Error in EnableEventViewer thread: " + ex.Message);
                }
            });

          //  IFreezeService.Log("Starting new thread for Event Viewer...");
            Thread th = new Thread(thstart);
            th.Start();
           // IFreezeService.Log("Thread started for EnableEventViewer");
        }

        public async void DisableEventViewer()
        {
           // await IFreezeService.Log("DisableEventViewer invoked");

            Process[] processes = Process.GetProcessesByName(ViewerProcessName);
          //  await IFreezeService.Log($"Found {processes.Length} processes with name {ViewerProcessName}");

            foreach (var process in processes)
            {
                try
                {
                   // await IFreezeService.Log($"Attempting to kill process ID: {process.Id}, Name: {process.ProcessName}");
                    process.Kill();
                  //  await IFreezeService.Log("Event Viewer has been stopped.");
                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to stop Event Viewer (ID: {process.Id}): {ex.Message}");
                }
            }

           // await IFreezeService.Log("DisableEventViewer finished");
        }

    }
}
