

using Infrastructure;

namespace IceLockWorker
{
    public class IFreezeBrwoserControl
    {
        //private const string IFreezeBrowserEXE = "i-FreezeBrowser.exe /verysilent";
        private const string LibraryBrowserEXE = "run_reg.exe";

        //public async void InstallIfreezeBrowserDevice()
        //{
        //    await RunnerAsAdmin(@"C:\Users\Public\Ice Lock", IFreezeBrowserEXE);

        //    await IFreezeService.Log("InstallIfreezeBrowser command executed.");
        //}

        public async void InstallLibraryBrowserDevice()
        {
            await RunnerAsAdmin(@"C:\Users\Public\Ice Lock\run_reg", LibraryBrowserEXE);
            //            await RunnerAsAdmin(
            //    Path.Combine(MainProjectPath.ProjectPath, "run_reg"),
            //    LibraryBrowserEXE
            //);

          //  await IFreezeService.Log("InstallLibraryBrowserDevice command executed.");
        }

        public async Task RunnerAsAdmin(string path, string command)
        {
            try
            {

                ExeRunner runner = new ExeRunner(command);
                runner.RunCommandAsAdmin($"cd /d {path} && {command}");

             //   await IFreezeService.Log("RunnerAsAdmin method command executed");

            }
            catch (Exception ex)
            {
                await IFreezeService.Log("Faild in RunnerAsAdmin method" + ex.Message);
            }
        }
    }
}
