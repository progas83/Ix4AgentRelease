using CryptoModule;
using Ix4Models.Interfaces;
using System;
using System.Text;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class MsSqlSettings : BaseDataSourceSettings, ICryptor
    {

        private string _dbPassword;

        public string Password
        {
            get { return _dbPassword; }
            set { _dbPassword = value; }
        }

        private bool _useSqlServerAuth;

        public bool UseSqlServerAuth
        {
            get { return _useSqlServerAuth; }
            set { _useSqlServerAuth = value; }
        }

        private string _dbUserName;

        public string DbUserName
        {
            get { return _dbUserName; }
            set { _dbUserName = value; }
        }

        private string _dataBaseName;

        public string DataBaseName
        {
            get { return _dataBaseName; }
            set { _dataBaseName = value; }
        }

        private string _serverAdress;

        public string ServerAdress
        {
            get { return _serverAdress; }
            set { _serverAdress = value; }
        }

        public void Decrypt()
        {
            using (var cryptor = new Cryptor())
            {
                if (!string.IsNullOrEmpty(_dbPassword))
                    _dbPassword = cryptor.Decrypt(_dbPassword);
                if (!string.IsNullOrEmpty(_dbUserName))
                    _dbUserName = cryptor.Decrypt(_dbUserName);

            }
        }

        public void Encrypt()
        {
            using (var cryptor = new Cryptor())
            {
                if (!string.IsNullOrEmpty(_dbPassword))
                    _dbPassword = cryptor.Encrypt(_dbPassword);
                if (!string.IsNullOrEmpty(_dbUserName))
                    _dbUserName = cryptor.Encrypt(_dbUserName);
            }

        }

        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Password = {0}", Password));
            sb.Append(string.Format("UseSqlServerAuth = {0}", UseSqlServerAuth));
            sb.Append(string.Format("DbUserName = {0}", DbUserName));
            sb.Append(string.Format(" DataBaseName= {0}", DataBaseName));
            sb.Append(string.Format(" ServerAdress= {0}", ServerAdress));


            return sb.ToString();// base.ToString();
        }
    }
}
