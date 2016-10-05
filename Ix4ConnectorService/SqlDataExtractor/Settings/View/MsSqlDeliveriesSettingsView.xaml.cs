﻿using System.Windows.Controls;
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
