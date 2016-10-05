using Ix4Models.SettingsDataModel;
using Ix4ServiceConfigurator.ViewModel.MsSql;
using System.Windows.Controls;

namespace Ix4ServiceConfigurator.View.MsSql
{
    /// <summary>
    /// Interaction logic for MsSqlDeliveriesSettingsView.xaml
    /// </summary>
    public partial class MsSqlDeliveriesSettingsView : UserControl
    {
        private MsSqlDeliveriesSettingsView()
        {
            InitializeComponent();
        }

        public MsSqlDeliveriesSettingsView(MsSqlDeliveriesSettings msSqlDeliveriesSettings) : this()
        {
            this.DataContext = new MsSqlDeliveryViewModel(msSqlDeliveriesSettings);
        }
    }
}
