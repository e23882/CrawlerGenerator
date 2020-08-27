namespace BeautyJson.ViewModel
{
    public class ShowJsonViewModel:ViewModelBase
    {
        #region Declarations
        private string _Result = string.Empty;
        #endregion

        #region Property
        public string Result
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}