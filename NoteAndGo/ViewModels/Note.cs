using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Data.Linq;

namespace NoteAndGo.Data
{
    [Table]
    public class Note : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public static int TITLE_MAX_LENGTH = 35;

        public Note() { }

        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private bool _changed;

        [Column]
        public bool Changed
        {
            get
            {
                return _changed;
            }
            set
            {
                NotifyPropertyChanging("Changed");
                _changed = value;
                NotifyPropertyChanged("Changed");
            }
        }

        private string _content;

        [Column]
        public string Content {
            get
            {
                return _content;
            }
            set
            {
                NotifyPropertyChanging("Content");
                _content = value;
                NotifyPropertyChanged("Content");
            }
        }

        private string _tags;

        [Column]
        public string Tags {
            get
            {
                return _tags;
            }
            set
            {
                NotifyPropertyChanging("Tags");
                if (value != null)
                {
                    _tags = value.Replace("\n", "").Replace(" ", "");
                }
                else
                {
                    _tags = value;
                }
                NotifyPropertyChanged("Tags");
            }
        }

        private string _createDate;

        [Column]
        public string CreateDate {
            get
            {
                return _createDate;
            }
            set
            {
                NotifyPropertyChanging("CreateDate");
                _createDate = value;
                NotifyPropertyChanged("CreateDate");
            }
        }

        private string _modifyDate;

        [Column]
        public string ModifyDate {
            get
            {
                return _modifyDate;
            }
            set
            {
                NotifyPropertyChanging("ModifyDate");
                _modifyDate = value;
                NotifyPropertyChanged("ModifyDate");
            }
        }

        private long _deleted;

        [Column]
        public long Deleted {
            get
            {
                return _deleted;
            }
            set
            {
                NotifyPropertyChanging("Deleted");
                _deleted = value;
                NotifyPropertyChanged("Deleted");
            }
        }

        private int _version;

        [Column]
        public int Version {
            get
            {
                return _version;
            }
            set
            {
                NotifyPropertyChanging("Version");
                _version = value;
                NotifyPropertyChanged("Version");
            }
        }

        private int _syncnum;

        [Column]
        public int Syncnum {
            get
            {
                return _syncnum;
            }
            set
            {
                NotifyPropertyChanging("Syncnum");
                _syncnum = value;
                NotifyPropertyChanged("Syncnum");
            }
        }

        private string _key;

        [Column]
        public string Key {
            get
            {
                return _key;
            }
            set
            {
                NotifyPropertyChanging("Key");
                _key = value;
                NotifyPropertyChanged("Key");
            }
        }

        public string GetTitle(string text)
        {
            if (text != null && text.Length > 0 && text.Length > Note.TITLE_MAX_LENGTH)
            {
                if (text.IndexOf("\n") > 0 && text.IndexOf("\n") > Note.TITLE_MAX_LENGTH)
                {
                    return text.Substring(0, text.IndexOf("\n"));
                }
                else
                {
                    return text;
                }
            }
            else
            {
                return text;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

    }
}
