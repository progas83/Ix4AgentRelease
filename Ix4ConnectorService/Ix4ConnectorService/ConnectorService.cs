using ConnectorWorkflowManager;
using Ix4Models;
using System.ServiceProcess;

namespace Ix4ConnectorService
{
    public class ConnectorService : ServiceBase
    {
        public ConnectorService()
        {
            AutoLog = true;
            CanPauseAndContinue = true;
            CanStop = true;
            ServiceName = CurrentServiceInformation.ServiceName;
        }

        protected override void OnStart(string[] args)
        {
            WorkflowManager.Instance.Start();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            WorkflowManager.Instance.Stop();
            base.OnStop();
        }

        protected override void OnContinue()
        {
            WorkflowManager.Instance.Continue();
            base.OnContinue();
        }

        protected override void OnPause()
        {
            WorkflowManager.Instance.Pause();
            base.OnPause();
        }
    }
}
