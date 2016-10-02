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
