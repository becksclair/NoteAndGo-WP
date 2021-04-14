using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace SimpleScribe
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void feedbackBtn_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.Subject = "SimpleScribe feedback";
            email.To = "feedback@steelcode.net";
            email.Show();
        }

        private void steelcodeLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webTask = new WebBrowserTask();
            webTask.Uri = new Uri(steelcodeLink.Text, UriKind.Absolute);
            webTask.Show();
        }

        private void jsonNetLink_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webTask = new WebBrowserTask();
            webTask.Uri = new Uri(jsonNetLink.Text, UriKind.Absolute);
            webTask.Show();
        }
    }
}