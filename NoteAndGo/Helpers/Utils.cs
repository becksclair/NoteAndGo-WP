using System;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using System.Xml.Linq;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace NoteAndGo
{
    public class Utils
    {
        public static bool InternetIsAvailable()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }
            return true;
        }

        public static void RemindReview()
        {
            //IsolatedStorageSettings.ApplicationSettings["numLaunches"] = 0;
            //IsolatedStorageSettings.ApplicationSettings["nextReminderOnLaunch"] = 0;

            bool ratedYet = false;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<bool>("ratedYet", out ratedYet);

            int nextReminderOnLaunch = 0;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<int>("nextReminderOnLaunch", out nextReminderOnLaunch);

            if (!ratedYet)
            {
                int numLaunches = 0;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<int>("numLaunches", out numLaunches);

                if (nextReminderOnLaunch == 0 || nextReminderOnLaunch == numLaunches)
                {
                    var ratingPrompt = new MessagePrompt { Title = "Like it so far?", Message = "Rate us on the marketplace, it encourages us to work harder and make this app more awesome." };
                    Button rateButton = new Button { Content = "rate" };
                    Button laterButton = new Button { Content = "later" };
                    rateButton.Click += (s, args) =>
                    {
                        IsolatedStorageSettings.ApplicationSettings["ratedYet"] = true;
                        var m = new MarketplaceDetailTask
                        {
                            ContentIdentifier = Utils.GetId().ToString(),
                            ContentType = MarketplaceContentType.Applications
                        };

                        m.Show();
                    };
                    laterButton.Click += (s, args) =>
                    {
                        IsolatedStorageSettings.ApplicationSettings["nextReminderOnLaunch"] = numLaunches + 5;
                        ratingPrompt.Hide();
                    };
                    ratingPrompt.ActionPopUpButtons.Clear();
                    ratingPrompt.ActionPopUpButtons.Add(rateButton);
                    ratingPrompt.ActionPopUpButtons.Add(laterButton);
                    ratingPrompt.Show();
                }
                numLaunches++;
                IsolatedStorageSettings.ApplicationSettings["numLaunches"] = numLaunches;
            }
        }

        public static Guid GetId()
        {
            Guid applicationId = Guid.Empty;
            var productId = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("ProductID");

            if (productId != null && !string.IsNullOrEmpty(productId.Value))
                Guid.TryParse(productId.Value, out applicationId);

            return applicationId;
        }
    }
}
