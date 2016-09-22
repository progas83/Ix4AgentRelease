using Ix4Connector.Ix4WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ix4Connector.Args
{
    public delegate void ExportDataAsyncCompletedEventHandler(object sender, ExportDataAsyncCompletedEventArgs e);

    public class ExportDataAsyncCompletedEventArgs : EventArgs
    {
        
        public ExportDataAsyncCompletedEventArgs(ExportDataCompletedEventArgs e) 
        {
            this.Result = e.Result;
            this.Error = e.Error;
            this.Cancelled = e.Cancelled;
            this.UserState = e.UserState;
        }
        public XmlNode Result { get; private set; }

        public Exception Error { get; private set; }

        public bool Cancelled { get; private set; }

        public object UserState { get; private set; }

    }
}
