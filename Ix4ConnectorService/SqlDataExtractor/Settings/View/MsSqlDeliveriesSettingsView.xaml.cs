using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ix4Models.SettingsDataModel;
using SqlDataExtractor.Settings.ViewModel;

namespace SqlDataExtractor.Settings.View
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
