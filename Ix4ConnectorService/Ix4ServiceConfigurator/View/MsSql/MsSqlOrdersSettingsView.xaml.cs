using Ix4Models.SettingsDataModel;
using Ix4ServiceConfigurator.ViewModel.MsSql;
using System.Windows.Controls;

namespace Ix4ServiceConfigurator.View.MsSql
{
    /// <summary>
    /// Interaction logic for MsSqlOrdersSettingsView.xaml
    /// </summary>
    public partial class MsSqlOrdersSettingsView : UserControl
    {
        private MsSqlOrdersSettingsView()
        {
            InitializeComponent();
        }

        public MsSqlOrdersSettingsView(MsSqlOrdersSettings msSqlOrdersSettings) : this()
        {
            this.DataContext = new MsSqlOrdersViewModel(msSqlOrdersSettings);
        }
    }
}
