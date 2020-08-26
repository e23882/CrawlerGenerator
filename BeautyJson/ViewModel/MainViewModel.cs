using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BeautyJson.Command;
using BeautyJson.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeautyJson.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Declarations

        private string _Json = string.Empty;
        private Root myDeserializedClass = null;
        private ObservableCollection<Entry> _DataCollection = new ObservableCollection<Entry>();
        private bool _FilterContentType = false;
        private string _FilterRule = string.Empty;

        #endregion

        #region Property

        public string Json
        {
            get { return _Json; }
            set
            {
                _Json = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BeautyJsonClickCommand
        {
            get { return new RelayCommand(BeautyJsonClickCommandAction); }
        }

        public ObservableCollection<Entry> DataCollection
        {
            get
            {
                if (FilterContentType == true && !string.IsNullOrEmpty(FilterRule))
                {
                    ObservableCollection<Entry> temp = new ObservableCollection<Entry>(_DataCollection.Where(x =>
                        (x.response.content.mimeType == "application/javascript" ||
                         x.response.content.mimeType == "application/json" ||
                         x.response.content.mimeType == "text/html") && 
                        x.response.content.text.IndexOf(FilterRule)!=-1
                        ));
                    return temp;
                }
                else if (FilterContentType == true)
                {
                    ObservableCollection<Entry> temp = new ObservableCollection<Entry>(_DataCollection.Where(x =>
                        x.response.content.mimeType == "application/javascript" ||
                        x.response.content.mimeType == "application/json" ||
                        x.response.content.mimeType == "text/html"));
                    return temp;
                }
                else
                    return _DataCollection;
            }
        }

        public RelayCommand ConnectionClickCommand
        {
            get { return new RelayCommand(ConnectionClickCommandAction); }
        }

        public string FilterRule
        {
            get { return _FilterRule; }
            set
            {
                _FilterRule = value;
                OnPropertyChanged("DataCollection");
                OnPropertyChanged();
            }
        }

        public bool FilterContentType
        {
            get { return _FilterContentType; }
            set
            {
                _FilterContentType = value;
                OnPropertyChanged("DataCollection");
                OnPropertyChanged();
            }
        }

        #endregion

        #region Memberfunction

        public MainViewModel()
        {
        }

        #region CommandAction

        private void ConnectionClickCommandAction(object obj)
        {
            var parameter = obj as string;
            if (parameter is null)
                return;
            var dt = DataCollection.Where(x => x.connection == parameter).FirstOrDefault();
            var jsonText = JsonConvert.SerializeObject(dt, Formatting.Indented);
            MessageBox.Show(jsonText);
        }

        private void BeautyJsonClickCommandAction(object obj)
        {
            JObject jo = JObject.Parse(this.Json);
            myDeserializedClass = JsonConvert.DeserializeObject<Root>(Json);
            foreach (var item in myDeserializedClass.log.entries)
            {
                DataCollection.Add(item);
            }
        }

        #endregion

        #endregion
    }
}