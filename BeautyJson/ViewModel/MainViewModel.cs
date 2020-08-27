using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BeautyJson.Command;
using BeautyJson.DataModel;
using BeautyJson.View;
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

        public RelayCommand ExportCommand
        {
            get
            {
                return new RelayCommand(ExportCommandAction);
            }
        }

        private void ExportCommandAction(object obj)
        {
            string result = string.Empty;
            string runQuery = string.Empty;
            List<FunctionDataModel> ExistFunctionName = new List<FunctionDataModel>();
            
            result += "import requests\r\n\r\n";
            
            foreach (var item in DataCollection.Where(x=>!string.IsNullOrEmpty(x.connection)))
            {
                var count = ExistFunctionName.Where(x => x.Connection == item.connection).Count();
                if (item.request.method == "Get")
                {
                    if (count == 0)
                    {
                        result += GenerateGetScript(item, "");    
                        ExistFunctionName.Add(new FunctionDataModel()
                        {
                            Connection = item.connection,
                            FunctionName = item.connection
                        });
                    }
                    else
                    {
                        result += GenerateGetScript(item, $"_{count}");
                        ExistFunctionName.Add(new FunctionDataModel()
                        {
                            Connection = item.connection,
                            FunctionName = item.connection+"_{count+1}"
                        });
                    }
                    
                }
                else
                {
                    if (count == 0)
                    {
                        result += GeneratePostScript(item, "");    
                        ExistFunctionName.Add(new FunctionDataModel()
                        {
                            Connection = item.connection,
                            FunctionName = item.connection
                        });
                    }
                    else
                    {
                        result += GeneratePostScript(item, $"_{count}");
                        ExistFunctionName.Add(new FunctionDataModel()
                        {
                            Connection = item.connection,
                            FunctionName = item.connection+"_{count+1}"
                        });
                    }
                }
                
                if (count == 0)
                    runQuery += $"    Connection{item.connection}()\r\n";
                else
                    runQuery += $"    Connection{item.connection}_{count}()\r\n";
            }

            result += "if __name__ == '__main__':\r\n"+runQuery;
            try
            {
                var ViewModel = new ShowJsonViewModel();
                ViewModel.Result = result;
                UcShowJson showJsonWindow = new UcShowJson();
                showJsonWindow.Title = "Output";
                showJsonWindow.DataContext = ViewModel; 
                showJsonWindow.Show();
            }
            catch (Exception ie)
            {
                
            }
            
        }

        public string GenerateGetScript(Entry parameter, string subFunctionName)
        {
            string result = string.Empty;
            result += $"def Connection{parameter.connection}{subFunctionName}():\r\n";
            result += $"    url = '{parameter.request.url}'\r\n";
            result += "    data = "+"{"+$""+"}"+"\r\n";
            result += "    headers = "+"{"+$""+"}"+"\r\n";
            result += "    response = requests.get(url, headers=headers, data=data)\r\n";
            result += "    # globalCookie = response.cookies.get_dict()\r\n";
            result += "    response.encoding = 'big5'\r\n\r\n";
            result += "    if response.status_code == 200:\r\n";
            result += $"        print('{parameter.connection} ok')\r\n";
            result += "    else:\r\n";
            result += $"        print('{parameter.connection} fail')\r\n\r\n";
            return result;
        }
        public string GeneratePostScript(Entry parameter, string subFunctionName)
        {
            string result = string.Empty;
            result += $"def Connection{parameter.connection}{subFunctionName}():\r\n";
            result += $"    url = '{parameter.request.url}'\r\n";
            result += "    params = "+"{"+$""+"}"+"\r\n";
            result += "    headers = "+"{"+$""+"}"+"\r\n";
            result += "    response = requests.post(url, headers=headers, params=params)\r\n";
            result += "    # globalCookie = response.cookies.get_dict()\r\n";
            result += "    response.encoding = 'big5'\r\n\r\n";
            result += "    if response.status_code == 200:\r\n";
            result += $"        print('{parameter.connection} ok')\r\n";
            result += "    else:\r\n";
            result += $"        print('{parameter.connection} fail')\r\n\r\n";
            return result;
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
            ShowJsonResult(jsonText, parameter);
        }

        public bool ShowJsonResult(string parameter, string connectionID)
        {
            try
            {
                var ViewModel = new ShowJsonViewModel();
                ViewModel.Result = parameter;
                UcShowJson showJsonWindow = new UcShowJson();
                showJsonWindow.Title = connectionID;
                showJsonWindow.DataContext = ViewModel; 
                showJsonWindow.Show();
                return true;
            }
            catch (Exception ie)
            {
                return false;
            }
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