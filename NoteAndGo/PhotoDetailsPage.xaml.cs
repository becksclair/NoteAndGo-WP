using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone;
using Microsoft.Phone.Controls;

namespace NoteAndGo
{
    public partial class PhotoDetailsPage : PhoneApplicationPage
    {
        public int Index { get; set; }
        public ImageModel CurrentItem { get; set; }

        private WriteableBitmap bitmap;

        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);

        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;

        public PhotoDetailsPage()
        {
            InitializeComponent();
        }

        private void DetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentItem = App.ViewModelImages.Items[Index];
            DataContext = CurrentItem;

            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForApplication();
            if (f.FileExists(CurrentItem.Source))
            {
                LoadImage(CurrentItem);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                Index = int.Parse(selectedIndex);
            }
        }

        private void LoadImage(ImageModel model)
        {
            try
            {
                IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForApplication();
                IsolatedStorageFileStream fileStream = f.OpenFile(model.Source, FileMode.Open, FileAccess.Read);
                bitmap = PictureDecoder.DecodeJpeg(fileStream);
                ImageView.Source = bitmap;
            }
            catch (IsolatedStorageException)
            {
                LoadImage(model);
            }
        }

        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(ImageView, 0);
            _oldFinger2 = e.GetPosition(ImageView, 1);
            _oldScaleFactor = 1;

        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;

            var currentFinger1 = e.GetPosition(ImageView, 0);
            var currentFinger2 = e.GetPosition(ImageView, 1);

            var translationDelta = GetTranslationDelta(currentFinger1, currentFinger2, _oldFinger1, _oldFinger2, ImagePosition, scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            if (scaleFactor < 2.0 && scaleFactor > 0.8)
            {
                UpdateImage(scaleFactor, translationDelta);
            } 
        }

        private void OnDragStarted(object sender, DragStartedGestureEventArgs e)
        {
            var image = sender as Image;
            if (image == null) return;
            var transform = image.RenderTransform as CompositeTransform;
            if (transform == null) return;
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var image = sender as Image;
            if (image == null) return;
            var transform = image.RenderTransform as CompositeTransform;
            if (transform == null) return;

            transform.CenterX = (transform.CenterX - e.HorizontalChange);
            transform.CenterY = (transform.CenterY - e.VerticalChange);
        }

        private void UpdateImage(double scaleFactor, Point delta)
        {
            TotalImageScale *= scaleFactor;
            if (TotalImageScale > 4.0 || TotalImageScale < 0.8) return;
            ImagePosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            var transform = (CompositeTransform) ImageView.RenderTransform;
            transform.ScaleX = TotalImageScale;
            transform.ScaleY = TotalImageScale;
            transform.TranslateX = ImagePosition.X;
            transform.TranslateY = ImagePosition.Y;
        }

        private Point GetTranslationDelta(Point currentFinger1, Point currentFinger2, Point oldFinger1,
            Point oldFinger2, Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
                                    currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
                                    currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point((newPos1.X + newPos2.X) / 2,
                                   (newPos1.Y + newPos2.Y) / 2);

            return new Point(newPos.X - currentPosition.X, newPos.Y - currentPosition.Y);
        }

        private void ApplicationBarDelete_Click(object sender, System.EventArgs e)
        {
            App.ViewModelImages.RemoveItem(CurrentItem);
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}