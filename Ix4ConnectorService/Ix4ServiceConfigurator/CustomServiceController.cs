using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Ix4ServiceConfigurator
{
    class CustomServiceController : ServiceController
    {
        public CustomServiceController(string serviceName) : base(serviceName)
        {

        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ServiceController sc = obj as ServiceController;
            if (sc == null)
                return false;
            try
            {
                return this.ServiceName.Equals(sc.ServiceName) && this.MachineName.Equals(sc.MachineName);
            }
            catch
            {
                return false;
            }
            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
