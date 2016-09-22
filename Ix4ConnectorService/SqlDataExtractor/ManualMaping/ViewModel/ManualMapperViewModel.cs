using Ix4Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlDataExtractor.ManualMaping.ViewModel
{
    public class ManualMapperViewModel : INotifyPropertyChanged
    {

        public ManualMapperViewModel()
        {
            MappedDictionary = new Dictionary<string, string>();
            int i = 0;
            foreach (var item in Ix4ComplexModel.GetPropertiesList())
            {
                MappedDictionary.Add(item.Name, i.ToString());
                i++;
            }
            Ix4Properties = Ix4ComplexModel.GetPropertiesList();
            SourceMapCollection = new ObservableCollection<DataSourceInfo>(
                (new SqlTableHeaderExplorer(CurrentServiceInformation.MsSqlDatabaseConnectionStringWindowsAuth)).
                GetTabelHeader("CurrentServiceInformation.MsSqlDataTableName"));
        }

        private DataSourceInfo _SelItem;

        public DataSourceInfo SelItemValue
        {
            get { return _SelItem; }
            set { _SelItem = value; }
        }

        private string _SelVal;

        public string SelVal
        {
            get { return _SelVal; }
            set
            {
                _SelVal = value;
                ServeMappedDictionary(_selKey, _SelVal);

            }
        }
        private void ServeMappedDictionary(string key, string val)
        {
            MappedDictionary[key] = val;
            OnPropertyChanged("MappedDictionary");
            //  SourceMapCollection.Remove(_SelItem);
        }
        private string _selKey;

        public string SelectedKey
        {
            get { return _selKey; }
            set { _selKey = value; }
        }

        private Dictionary<string, string> _mappedDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> MappedDictionary
        {
            get
            {
                return _mappedDictionary;
            }
            set
            {
                _mappedDictionary = value;
            }
        }

        public PropertyInfo[] Ix4Properties
        {
            get; private set;
        }

        public ObservableCollection<DataSourceInfo> _sourceMapCollection = new ObservableCollection<DataSourceInfo>();
        public ObservableCollection<DataSourceInfo> SourceMapCollection
        {
            get { return _sourceMapCollection; }
            private set { _sourceMapCollection = value; }
        }

        private DataSourceInfo _selectedMappedData;
        public DataSourceInfo SelectedMappedData
        {
            get { return _selectedMappedData; }
            set { _selectedMappedData = value; }
        }
        private ObservableCollection<DataSourceInfo> AlreadyMappedItems = new ObservableCollection<DataSourceInfo>();

        private void ServeDataSourceInfoCollection(DataSourceInfo selectedMappedData)
        {
            SourceMapCollection = (ObservableCollection<DataSourceInfo>)SourceMapCollection.Except(AlreadyMappedItems);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
