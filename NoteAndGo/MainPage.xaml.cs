using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using NoteAndGo.Data;
using NoteAndGo.simplenote;
using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NoteAndGo
{
    public partial class MainPage : PhoneApplicationPage
    {
        public ImageModel CurrentImageModel { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            PhotosList.DataContext = App.ViewModelImages;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.ViewModel.GetNotes();
            App.ViewModelImages.LoadData();
            Utils.RemindReview();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Refresh();
            base.OnNavigatedTo(e);
        }

        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesList.SelectedIndex == -1) return;
            Note n = NotesList.SelectedItem as Note;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + n.Id, UriKind.Relative));
            NotesList.SelectedIndex = -1;
        }

        private void PhotosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhotosList.SelectedIndex == -1) return;
            NavigationService.Navigate(new Uri("/PhotoDetailsPage.xaml?selectedItem=" + PhotosList.SelectedIndex, UriKind.Relative));
            PhotosList.SelectedIndex = -1;
        }

        private void ApplicationBarAdd_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DetailsPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarCapture_Click(object sender, EventArgs e)
        {
            try
            {
                CameraCaptureTask CaptureTask = new CameraCaptureTask();
                CaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
                CaptureTask.Show();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("An error occurred. " + ex.Message);
            }
        }

        private void ApplicationBarSearch_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/DetailsPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void ApplicationBarMenuSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private static void Refresh()
        {
            AppSettings settings = new AppSettings();
            if (settings.GetValueOrDefault("SNEnabled", false))
            {
                App.simpleNote = new Simplenote(App.ViewModel);
            }
        }

        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                string filename = "temp.jpg";
                // Create virtual store and file stream. Check for duplicate temp files.
                var store = IsolatedStorageFile.GetUserStoreForApplication();
                if (store.FileExists(filename))
                {
                    store.DeleteFile(filename);
                }

                IsolatedStorageFileStream fileStream = store.CreateFile(filename);
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(e.ChosenPhoto);

                WriteableBitmap wb = new WriteableBitmap(bitmap);

                // Encode WriteableBitmap object to a JPEG stream.
                Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                fileStream.Close();

                CurrentImageModel = new ImageModel { Source = filename, Title = filename };
                App.ViewModelImages.AddItem(CurrentImageModel);

                InputPrompt input = new InputPrompt();
                input.Completed += new EventHandler<PopUpEventArgs<string, PopUpResult>>(editInput_Completed);
                input.Title = "Give this note a title";
                input.Show();
            }
            else
            {
                MessageBox.Show("Error opening camera");
            }
        }

        void editInput_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.Result.CompareTo("") == 0)
            {
                return;
            }
            CurrentImageModel.Title = e.Result;
            App.ViewModelImages.EditItem(CurrentImageModel);
        }
    }
}