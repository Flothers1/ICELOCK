using i_freeze.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using i_freeze.Commands.UpgradeMessageBoxCommands;
using Infrastructure.DataContext;
using Infrastructure;

namespace i_freeze.Commands.HomeCommands
{
    public class SecurityUpdateCommand : CommandBase
    {
        public async override void Execute(object parameter)
        {
            using (var context = new ActivateDLPContext())
            {
                try
                {
                    
                    var email = context.ActivateDLPEntity.Select(x => x.Email).FirstOrDefault();
                    DeviceManagement deviceManagement = new DeviceManagement();
                    var serviceControl = new WindowsServiceControl();
                    await serviceControl.BlockDLPProcess();
                    await serviceControl.RunDLP_PM();
                    deviceManagement.RetrieveDeviceConfigurations(email);


                    
                }
                catch (Exception ex)
                {
                    await DeviceManagement.SendLogs("SecurityUpdateCommand Class Execute Method: ", ex.Message);
                   
                }
                 


            }
        }
    }
}

