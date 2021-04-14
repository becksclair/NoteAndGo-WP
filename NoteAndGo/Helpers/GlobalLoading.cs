using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace NoteAndGo.Helpers
{
    public class GlobalLoading : INotifyPropertyChanged
    {
        private ProgressIndicator _indicator;
        public string Text
        {
            get
            {
                return _indicator.Text;
            }
            set
            {
                _indicator.Text = value;
            }
        }

        private GlobalLoading()
        {
        }

        public void Initialize(PhoneApplicationFrame frame)
        {
            // If using AgFx:
            // DataManager.Current.PropertyChanged += OnDataManagerPropertyChanged;

            _indicator = new ProgressIndicator();
            frame.Navigated += OnRootFrameNavigated;
        }

        public void SetLoading(bool val, string text)
        {
#if DEBUG
            Text = text;
#endif
            IsLoading = val;
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Use in Mango to share a single progress indicator instance.
            var ee = e.Content;
            var page = ee as PhoneApplicationPage;
            if (page != null)
            {
                page.SetValue(SystemTray.ProgressIndicatorProperty, _indicator);
            }
        }

        private void OnDataManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsLoading" == e.PropertyName)
            {
                // if AgFx: IsDataManagerLoading = DataManager.Current.IsLoading;
                NotifyValueChanged();
            }
        }

        private static GlobalLoading _in;
        public static GlobalLoading Instance
        {
            get
            {
                if (_in == null)
                {
                    _in = new GlobalLoading();
                }

                return _in;
            }
        }

        public bool IsDataManagerLoading { get; set; }

        public bool ActualIsLoading
        {
            get
            {
                return IsLoading || IsDataManagerLoading;
            }
        }

        private bool _loading;

        public bool IsLoading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;
                if (!value)
                {
                    _indicator.Text = "";
                }
                NotifyValueChanged();
            }
        }

        private void NotifyValueChanged()
        {
            if (_indicator != null)
            {
                _indicator.IsIndeterminate = _loading || IsDataManagerLoading;

                // for now, just make sure it's always visible.
                if (_indicator.IsVisible == false)
                {
                    _indicator.IsVisible = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
