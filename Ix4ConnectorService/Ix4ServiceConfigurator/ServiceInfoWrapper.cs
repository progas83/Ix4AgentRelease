using Ix4Models;
using Ix4ServiceConfigurator.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ix4ServiceConfigurator
{
    class ServiceInfoWrapper
    {

        private static object _padlock = new object();

        private static object _singletoneLocker = new object();
        private static ServiceInfoWrapper _serviceInfoWrapper = null;

        public static ServiceInfoWrapper Instance
        {
            get
            {
                if (_serviceInfoWrapper == null)
                {
                    lock (_singletoneLocker)
                    {
                        if (_serviceInfoWrapper == null)
                        {
                            _serviceInfoWrapper = new ServiceInfoWrapper();
                        }
                    }
                }
                return _serviceInfoWrapper;
            }
        }

        private ServiceInfoWrapper()
        {
        }

        private ServiceController CurrentService
        {
            get
            {
                return ServiceController.GetServices().FirstOrDefault(item => item.ServiceName.Equals(CurrentServiceInformation.ServiceName)); //_currentService;
            }
        }

        public ServiceControllerStatus ServiceStatus
        {
            get
            {
                return CurrentService != null ? CurrentService.Status : ServiceControllerStatus.Stopped;
            }
        }


        public bool ServiceExist
        {
            get
            {
                return CurrentService != null;
            }
        }

        public void StopService()
        {
            try
            {
                if (CurrentService != null)
                {
                    CurrentService.Stop();
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void StartService()
        {
            try
            {
                if (CurrentService != null)
                {
                    CurrentService.Start();
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void PauseService()
        {
            try
            {
                if (CurrentService != null)
                {
                    CurrentService.Pause();
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void ContinueService()
        {
            try
            {
                if (CurrentService != null)
                {
                    CurrentService.Continue();
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
