using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4ServiceConfigurator.ViewModel
{
    public interface IPwd
    {
        System.Security.SecureString PasswordGet { get; }
        void PasswordSet(string val);
    }
}
