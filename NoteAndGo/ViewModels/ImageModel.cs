using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NoteAndGo
{
    public class ImageModel : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get
            {
                if (_title.EndsWith(".jpg"))
                {
                    return _title.Substring(0, _title.LastIndexOf('.'));
                }
                else
                {
                    return _title;
                }
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _source;
        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                if (value != _source)
                {
                    _source = value;
                    NotifyPropertyChanged("Source");
                }
            }
        }

        private WriteableBitmap _bitmap;
        public WriteableBitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
            set
            {
                if (value != _bitmap)
                {
                    _bitmap = value;
                    NotifyPropertyChanged("Bitmap");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}