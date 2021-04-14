using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NoteAndGo.Data
{
    public class ViewModelNotes : INotifyPropertyChanged
    {
        private NotesContext _dataContext;
        private NotesContext DataContext
        {
            get
            {
                if (_dataContext != null)
                {
                    return _dataContext;
                }
                return _dataContext = new NotesContext(NotesContext.DBConnectionString);
            }
        }

        public IEnumerable<Note> _items;
        public IEnumerable<Note> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public void GetNotes()
        {
            Items = DataContext.Notes.OrderByDescending(x => x.ModifyDate).Where(x => x.Deleted == 0);
        }

        public Note GetNote(int index)
        {
            return DataContext.Notes.Where(x => x.Id == index).FirstOrDefault();
        }

        public void InsertNote(Note note)
        {
            DataContext.Notes.InsertOnSubmit(note);
            DataContext.SubmitChanges();
            GetNotes();
        }

        public void SaveChanges()
        {
            DataContext.SubmitChanges();
        }

        public void DeleteNote(Note note)
        {
            note.Deleted = 1;
            note.Version = note.Version + 1;
            DataContext.Notes.DeleteOnSubmit(note);
            DataContext.SubmitChanges();
            GetNotes();
        }

        public void FlushCache()
        {
            DataContext.DeleteDatabase();
        }

        public Note FindByKey(string key)
        {
            return DataContext.Notes.Where(x => x.Key == key).FirstOrDefault();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
