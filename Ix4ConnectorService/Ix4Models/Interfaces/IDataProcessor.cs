using Ix4Models.Reports;
using Ix4Models.SettingsDataModel;
using System;

namespace Ix4Models.Interfaces
{
    public interface IDataProcessor
    {
        event EventHandler<DataReportEventArgs> OperationReportEvent;
        void LoadSettings(CustomerInfo customerSettings);
        void ImportData();
        void ExportData();
    }
}
