using Ix4Models.CryptoModule;
using Ix4Models.Interfaces;
using System;

namespace Ix4Models.SettingsDataModel
{
    [Serializable]
    public class MsSqlSettings : BaseDataSourceSettings 
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

        public override void Decrypt()
        {
            using (var cryptor = new Cryptor())
            {
                if (!string.IsNullOrEmpty(_dbPassword))
                    _dbPassword = cryptor.Decrypt(_dbPassword);
                if (!string.IsNullOrEmpty(_dbUserName))
                    _dbUserName = cryptor.Decrypt(_dbUserName);

            }
        }

        public override void Encrypt()
        {
            using (var cryptor = new Cryptor())
            {
                if (!string.IsNullOrEmpty(_dbPassword))
                    _dbPassword = cryptor.Encrypt(_dbPassword);
                if (!string.IsNullOrEmpty(_dbUserName))
                    _dbUserName = cryptor.Encrypt(_dbUserName);
            }
        }
    }
}
