using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using NoteAndGo.Data;

namespace NoteAndGo
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        private bool IsNew { get; set; }
        private Note Current;

        public DetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                Current = App.ViewModel.GetNote(index);
                IsNew = false;
            }
            else
            {
                // Create a new note
                Current = new Note();
                IsNew = true;
            }
            DataContext = Current;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Save current note
            Current.Content = ContentText.Text;

            if (IsNew)
            {
                if (Current.Content != "")
                {
                    App.ViewModel.InsertNote(Current);
                }
            }
            else
            {
                if (Current.Content != "")
                {
                    Current.Version = Current.Version +1;
                    App.ViewModel.SaveChanges();
                }
                else
                {
                    Current.Version = Current.Version + 1;
                    Current.Deleted = 1;
                    App.ViewModel.SaveChanges();
                }
            }
            base.OnNavigatedFrom(e);
        }

        private void ApplicationBarDelete_Click(object sender, System.EventArgs e)
        {
            if (!IsNew)
            {
                Current.Version = Current.Version + 1;
                Current.Deleted = 1;
                App.ViewModel.SaveChanges();
            }
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ShareButton_Click(object sender, EventArgs e)
        {
            if (Current.Content != null)
            {
                EmailComposeTask email = new EmailComposeTask();
                email.Subject = "I'm sharing a note with you";
                email.Body = Current.Content;
                email.Show();
            }
            else
            {
                MessageBox.Show("Can't share an empty note");
            }
        }
    }
}