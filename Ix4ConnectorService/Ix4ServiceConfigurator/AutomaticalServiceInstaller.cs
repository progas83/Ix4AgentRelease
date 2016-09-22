using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ix4ServiceConfigurator
{
    internal class AutomaticalServiceInstaller
    {
        private static string servicePath = Ix4ConnectorService.Program.ServiceLocation;

        public static bool InstallService()
        {
            try
            {
                if(!ServiceInfoWrapper.Instance.ServiceExist)
                {
                    ManagedInstallerClass.InstallHelper(new string[] { servicePath});
                }
            }
            catch(Exception ex)
            {
                 return false;
            }
            return true;
        }

        public static bool UninstallService()
        {
            try
            {
                if(ServiceInfoWrapper.Instance.ServiceExist)
                {
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", servicePath });
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}
