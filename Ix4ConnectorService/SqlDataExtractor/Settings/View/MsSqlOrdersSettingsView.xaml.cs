﻿using System.Windows.Controls;
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
