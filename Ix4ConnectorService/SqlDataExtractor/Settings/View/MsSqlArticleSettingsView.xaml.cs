using System.Windows.Controls;
using Ix4Models.SettingsDataModel;
using SqlDataExtractor.Settings.ViewModel;

namespace SqlDataExtractor.Settings.View
{
    /// <summary>
    /// Interaction logic for MsSqlArticleSettingsView.xaml
    /// </summary>
    public partial class MsSqlArticleSettingsView : UserControl
    {
        private MsSqlArticlesSettings _msSqlArticlesSettings;

        private MsSqlArticleSettingsView()
        {
            InitializeComponent();
        }

        public MsSqlArticleSettingsView(MsSqlArticlesSettings msSqlArticlesSettings):this()
        {
           // this._msSqlArticlesSettings = msSqlArticlesSettings;

            this.DataContext = new MsSqlArticleViewModel(msSqlArticlesSettings);
        }
    }
}
