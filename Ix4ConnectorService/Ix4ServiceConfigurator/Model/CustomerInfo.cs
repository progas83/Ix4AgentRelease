using Ix4Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Ix4ServiceConfigurator.Model
{
    [Serializable]
    public class CustomerInfo
    {
        public CustomerInfo()
        {
            ServiceName = CurrentServiceInformation.ServiceName;
        }



        public string ServiceName { get; set; }
        public string UserName { get; set; }

        public int ClientID { get; set; }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string ServiceEndpoint { get; set; }

        public bool Default { get; set; }

        public void Decrypt(CryptoModule.Cryptor cryptor)
        {
            _password =cryptor.Decrypt(_password);// 
        }

        public void Encrypt(CryptoModule.Cryptor cryptor)
        {
            _password = cryptor.Encrypt(_password);
        }

        #region PluginsSettings
        public PluginsSettings PluginSettings { get; set; }
        #endregion
    }
}
