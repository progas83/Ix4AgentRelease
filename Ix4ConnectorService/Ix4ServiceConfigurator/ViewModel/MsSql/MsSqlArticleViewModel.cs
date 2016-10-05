using Ix4Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ix4Models.SettingsDataModel;

namespace Ix4ServiceConfigurator.ViewModel.MsSql
{
     public class MsSqlArticleViewModel : BaseViewModel
    {
        private MsSqlArticlesSettings _msSqlArticlesSettings;

        public MsSqlArticleViewModel(MsSqlArticlesSettings msSqlArticlesSettings)
        {
            _msSqlArticlesSettings = msSqlArticlesSettings;
        }

        public string ArticlesQuery
        {
            get
            {
                return _msSqlArticlesSettings.ArticlesQuery;
            }
            set
            {
                _msSqlArticlesSettings.ArticlesQuery = value;
                OnPropertyChanged("ArticlesQuery");
            }
        }
    }
}
