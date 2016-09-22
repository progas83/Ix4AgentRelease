using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ix4Models
{
    public class MappedItem : INotifyPropertyChanged
    {
        private static List<string> SourceMapCollection = new List<string>();

        private static List<string> AlreadyMappedCollection = new List<string>();

        public static void InitializeMapCollection(IEnumerable<string> mapCollection)
        {
            SourceMapCollection = new List<string>(mapCollection);
        }

        public List<string> SourceCollection
        {
            get { return SourceMapCollection; }
        }

        public MappedItem(string baseValue, string mappedValue)
        {
            BaseValue = baseValue;
            MappedValue = mappedValue;
        }

        private string _baseValue;

        public string BaseValue
        {
            get { return _baseValue; }
            private set { _baseValue = value; }
        }

        private string _mappedValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public string MappedValue
        {
            get { return _mappedValue; }
            private set
            {
                _mappedValue = value;
                OnPropertyChanged("MappedValue");
            }
        }
        private string _selectedItemValue;
        public string SelItemValue
        {
            get
            {
                return _selectedItemValue;
            }
            set
            {
                if (value != _selectedItemValue && value != null)
                {
                    ManageCollections(value);
                    _selectedItemValue = value;
                    MappedValue = value;
                }
            }
        }
        private void ManageCollections(string val)
        {

            if (_selectedItemValue != null)
            {
                AlreadyMappedCollection.Remove(_selectedItemValue);
                SourceMapCollection.Add(_selectedItemValue);
            }
            if (val != null)
            {
                AlreadyMappedCollection.Add(val);
                SourceMapCollection.Remove(val);
            }
            OnPropertyChanged("SourceCollection");
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
