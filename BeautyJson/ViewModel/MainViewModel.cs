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
        private string _FilterRule = string.Empty;

        private bool _FilterContentType = false;
        private bool _FilterDocument = false;
        private bool _FilterStyleSheet = false;
        private bool _FilterFont = false;
        private bool _FilterImage = false;
        private bool _FilterScript = false;
        private bool _FilterXHR = false;
        private bool _FilterOther = false;

        private Root myDeserializedClass = null;

        private ObservableCollection<Entry> _DataCollection = new ObservableCollection<Entry>();

        private Entry _CurrentItem = new Entry();

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

        public ObservableCollection<Entry> DataCollection
        {
            get
            {
                List<Entry> tempResult = _DataCollection.ToList();

                if (FilterDocument == true)
                    tempResult = tempResult.Where(x => x._resourceType != "document").ToList();
                if (FilterFont == true)
                    tempResult = tempResult.Where(x => x._resourceType != "font").ToList();
                if (FilterImage == true)
                    tempResult = tempResult.Where(x => x._resourceType != "image").ToList();
                if (FilterOther == true)
                    tempResult = tempResult.Where(x => x._resourceType != "other").ToList();
                if (FilterScript == true)
                    tempResult = tempResult.Where(x => x._resourceType != "script").ToList();
                if (FilterStyleSheet == true)
                    tempResult = tempResult.Where(x => x._resourceType != "stylesheet").ToList();
                if (FilterXHR == true)
                    tempResult = tempResult.Where(x => x._resourceType != "xhr").ToList();
                if (!string.IsNullOrEmpty(FilterRule))
                    tempResult = tempResult.Where(x => x.response.content.text.IndexOf(FilterRule) != -1).ToList();
                if (FilterDocument == false && FilterFont == false && FilterImage == false && FilterOther == false &&
                    FilterScript == false && FilterStyleSheet == false && FilterXHR == false &&
                    string.IsNullOrEmpty(FilterRule))
                    return _DataCollection;
                else
                    return new ObservableCollection<Entry>(tempResult);
            }
        }

        public RelayCommand BeautyJsonClickCommand
        {
            get { return new RelayCommand(BeautyJsonClickCommandAction); }
        }


        public Entry CurrentItem
        {
            get { return _CurrentItem; }
            set
            {
                _CurrentItem = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand ConnectionClickCommand
        {
            get { return new RelayCommand(ConnectionClickCommandAction); }
        }

        public RelayCommand ExportCommand
        {
            get { return new RelayCommand(ExportCommandAction); }
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

        public bool FilterDocument
        {
            get { return _FilterDocument; }
            set
            {
                _FilterDocument = value;
                OnPropertyChanged();
                OnPropertyChanged("DataCollection");
            }
        }

        public bool FilterStyleSheet
        {
            get { return _FilterStyleSheet; }
            set
            {
                _FilterStyleSheet = value;
                OnPropertyChanged();
                OnPropertyChanged("DataCollection");
            }
        }

        public bool FilterFont
        {
            get { return _FilterFont; }
            set
            {
                _FilterFont = value;
                OnPropertyChanged();
                OnPropertyChanged("DataCollection");
            }
        }

        public bool FilterImage
        {
            get { return _FilterImage; }
            set
            {
                _FilterImage = value;
                OnPropertyChanged();
                OnPropertyChanged("DataCollection");
            }
        }

        public bool FilterScript
        {
            get { return _FilterScript; }
            set
            {
                _FilterScript = value;
                OnPropertyChanged();
                OnPropertyChanged("DataCollection");
            }
        }

        public bool FilterXHR
        {
            get { return _FilterXHR; }
            set
            {
                _FilterXHR = value;
                OnPropertyChanged();
                OnPropertyChanged("DataCollection");
            }
        }

        public bool FilterOther
        {
            get { return _FilterOther; }
            set
            {
                _FilterOther = value;
                OnPropertyChanged();
                OnPropertyChanged("DataCollection");
            }
        }

        private void ExportCommandAction(object obj)
        {
            string result = string.Empty;
            string runQuery = string.Empty;
            List<FunctionDataModel> ExistFunctionName = new List<FunctionDataModel>();

            result += "import requests\r\n\r\n";

            foreach (var item in DataCollection.OrderBy(y => y.connection))
            {
                if (string.IsNullOrEmpty(item.connection))
                {
                    if (item.request.method == "Get")
                    {
                        result += GenerateGetScript(item, $"Temp{ExistFunctionName.Count+1}");
                        ExistFunctionName.Add(new FunctionDataModel()
                        {
                            Connection = item.connection,
                            FunctionName = item.connection
                        });
                    }
                    else
                    {
                        result += GeneratePostScript(item, $"Temp{ExistFunctionName.Count+1}");
                        ExistFunctionName.Add(new FunctionDataModel()
                        {
                            Connection = item.connection,
                            FunctionName = item.connection + "_{count+1}"
                        });
                    }

                    runQuery += $"    ConnectionTemp{ExistFunctionName.Count}()\r\n";
                }
                else
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
                                FunctionName = item.connection + "_{count+1}"
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
                                FunctionName = item.connection + "_{count+1}"
                            });
                        }
                    }

                    if (count == 0)
                        runQuery += $"    Connection{item.connection}()\r\n";
                    else
                        runQuery += $"    Connection{item.connection}_{count}()\r\n";
                }
            }

            result += "if __name__ == '__main__':\r\n" + runQuery;
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
            string headerString = string.Empty;
            foreach (var item in parameter.request.headers)
            {
                if(item.name!="Content-Length")
                    headerString += $"'{item.name}':'{item.value}',";
            }

            string result = string.Empty;
            result += $"def Connection{parameter.connection}{subFunctionName}():\r\n";
            result += $"    url = '{parameter.request.url}'\r\n";
            result += "    data = " + "{" + $"" + "}" + "\r\n";
            result += "    headers = " + "{" + $"" + "}" + "\r\n";
            result += "    response = requests.get(url, headers=headers, data=data, timeout = 3)\r\n";
            result += "    # globalCookie = response.cookies.get_dict()\r\n";
            result += "    response.encoding = 'big5'\r\n\r\n";
            result += "    if response.status_code == 200:\r\n";
            result += $"        print('{parameter.connection}{subFunctionName} ok')\r\n";
            result += "    else:\r\n";
            result += $"        print('{parameter.connection}{subFunctionName} fail')\r\n\r\n";
            return result;
        }

        public string GeneratePostScript(Entry parameter, string subFunctionName)
        {
            string headerString = string.Empty;
            string postDataString = string.Empty;
            //整理header資料
            foreach (var item in parameter.request.headers)
            {
                if(item.name!="Content-Length")
                    headerString += $"'{item.name}':'{item.value}',";
            }

            //整理params資料
            if(parameter.request.postData != null)
                postDataString = "'"+parameter.request.postData.text.Replace("=", "':'").Replace("&", "','").Replace("&", "','")+"'";
            
            string result = string.Empty;
            result += $"def Connection{parameter.connection}{subFunctionName}():\r\n";
            result += $"    url = '{parameter.request.url}'\r\n";
            result += "    params = " + "{" + postDataString + "}" + "\r\n";
            result += "    headers = " + "{" + headerString + "}" + "\r\n";
            result += "    response = requests.post(url, headers=headers, params=params, timeout = 3)\r\n";
            result += "    # globalCookie = response.cookies.get_dict()\r\n";
            result += "    response.encoding = 'big5'\r\n\r\n";
            result += "    if response.status_code == 200:\r\n";
            result += $"        print('{parameter.connection}{subFunctionName} ok')\r\n";
            result += "    else:\r\n";
            result += $"        print('{parameter.connection}{subFunctionName} fail')\r\n\r\n";
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
            var CurrentConnection = "No Connection";
            var jsonText = JsonConvert.SerializeObject(CurrentItem, Formatting.Indented);

            if (CurrentItem.connection != null)
                CurrentConnection = CurrentItem.connection;
            ShowJsonResult(jsonText, CurrentConnection);
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

            DataCollection.OrderBy(x => x.startedDateTime);
        }

        #endregion

        #endregion
    }
}