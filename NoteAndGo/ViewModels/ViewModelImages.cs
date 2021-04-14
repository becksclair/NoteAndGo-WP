using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using System.Windows;


namespace NoteAndGo
{
    public class ViewModelImages : INotifyPropertyChanged
    {
        public ViewModelImages()
        {
            this.Items = new ObservableCollection<ImageModel>();
        }

        public ObservableCollection<ImageModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            var files = store.GetFileNames();

            foreach (var item in files)
            {
                if (item.StartsWith("tempFile"))
                {
                    IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForApplication();
                    if (f.FileExists(item))
                    {
                        f.DeleteFile(item);
                    }
                    continue;
                }

                if (item.EndsWith("jpg"))
                {
                    App.ViewModelImages.Items.Add(new ImageModel { Source = item, Title = item, Bitmap = LoadImage(item) });
                }
            }

            this.IsDataLoaded = true;
        }

        private WriteableBitmap LoadImage(string file)
        {
            try
            {
                IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream fileStream = f.OpenFile(file, FileMode.Open, FileAccess.Read);
                return PictureDecoder.DecodeJpeg(fileStream);
            }
            catch (IsolatedStorageException ex)
            {
                return LoadImage(file);
            }
        }

        public void AddItem(ImageModel model)
        {
            model.Bitmap = LoadImage(model.Source);
            Items.Add(model);
        }

        public void EditItem(ImageModel model)
        {
            try
            {
                IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForApplication();
                f.MoveFile(model.Source, model.Title + ".jpg");
                Items.Clear();
                LoadData();
            }
            catch (IsolatedStorageException ex)
            {
                MessageBox.Show("Error saving notes");
            }
        }

        public void RemoveItem(ImageModel model)
        {
            if (DeleteFile(model.Source))
            {
                Items.Remove(model);
            }
        }

        private bool DeleteFile(string file)
        {
            try
            {
                IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForApplication();
                if (f.FileExists(file))
                {
                    f.DeleteFile(file);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (IsolatedStorageException ex)
            {
                return DeleteFile(file);
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