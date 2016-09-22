using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Ix4ServiceConfigurator.EventArguments
{
    public class ServiceControllerStatusEventArg : EventArgs
    {
        ServiceControllerStatus _newStastus;
        public ServiceControllerStatusEventArg(ServiceControllerStatus newStastus)
        {
            _newStastus = newStastus;
        }

        public ServiceControllerStatus NewStatus
        {
            get { return _newStastus; }
        }
    }
}
