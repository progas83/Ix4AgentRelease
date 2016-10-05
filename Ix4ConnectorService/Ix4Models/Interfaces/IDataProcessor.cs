using Ix4Models.SettingsDataModel;

namespace Ix4Models.Interfaces
{
    public interface IDataProcessor
    {
        void LoadSettings(CustomerInfo customerSettings);
        void ImportData();
        void ExportData();
    }
}
