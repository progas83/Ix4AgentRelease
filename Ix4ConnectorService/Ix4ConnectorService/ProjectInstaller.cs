using Ix4Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Ix4ConnectorService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceInstaller _serviceInstaller;
        private ServiceProcessInstaller _processInstaller;
        public ProjectInstaller()
        {
            InitializeComponent();

            _serviceInstaller = new ServiceInstaller();
            _serviceInstaller.ServiceName =  CurrentServiceInformation.ServiceName;//"NavisionService";//
            _serviceInstaller.Description = CurrentServiceInformation.ServiceDescription;
            _serviceInstaller.StartType = ServiceStartMode.Automatic;
            _serviceInstaller.AfterInstall += OnAfterServiceInstall;
            _serviceInstaller.BeforeInstall += OnBeforeServiceInstall;



            _processInstaller = new ServiceProcessInstaller();
            _processInstaller.Password = null;
            _processInstaller.Username = null;
            _processInstaller.Account = ServiceAccount.LocalSystem;

            Installers.Add(_serviceInstaller);
            Installers.Add(_processInstaller);
        }


        private void OnBeforeServiceInstall(object sender, InstallEventArgs e)
        {

            //throw new NotImplementedException();
        }

        private void OnAfterServiceInstall(object sender, InstallEventArgs e)
        {
            try
            {
                ServiceController sc = new ServiceController(CurrentServiceInformation.ServiceName);
                sc.Start();
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }
        
    }
}
