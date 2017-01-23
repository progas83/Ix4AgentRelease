using Ix4Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models.SettingsDataModel
{
   public class XmlFolderSettingsModel : BaseDataSourceSettings
    {
        public XmlFolderSettingsModel() : base()
        {

        }

        private string _xmlSuccessFolder;
        private string _xmlFailureFolder;
        private string _xmlItemSourceFolder;
        private bool _activateActionOnFailure;
        private bool _activateActionOnSuccess;
        private XmlResultHandleActions _actionOnFailure;
        private XmlResultHandleActions _actionOnSuccess;


      
        public string SuccessFolder
        {
            get { return _xmlSuccessFolder; }
            set { _xmlSuccessFolder = value; }
        }
  
        public string FailureFolder
        {
            get { return _xmlFailureFolder; }
            set { _xmlFailureFolder = value; }
        }
       
        public string XmlItemSourceFolder
        {
            get { return _xmlItemSourceFolder; }
            set { _xmlItemSourceFolder = value; }
        }
       
        public bool ActivateActionOnFailure
        {
            get { return _activateActionOnFailure; }
            set { _activateActionOnFailure = value; }
        }
        
        public bool ActivateActionOnSuccess
        {
            get { return _activateActionOnSuccess; }
            set { _activateActionOnSuccess = value; }
        }
        
        public XmlResultHandleActions ActionOnFailure
        {
            get { return _actionOnFailure; }
            set { _actionOnFailure = value; }
        }
        
        public XmlResultHandleActions ActionOnSuccess
        {
            get { return _actionOnSuccess; }
            set { _actionOnSuccess = value; }
        }

        public override void Decrypt()
        {
            //throw new NotImplementedException();
        }

        public override void Encrypt()
        {
            //throw new NotImplementedException();
        }
    }
}
