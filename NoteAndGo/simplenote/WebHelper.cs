using System;
using System.IO;
using System.Net;
using System.Windows;

namespace NoteAndGo.simplenote
{
    public static class WebHelper
    {
        public static string API_LOGIN_URL = "https://simple-note.appspot.com/api/login";    // POST
        public static string API_NOTES_URL = "https://simple-note.appspot.com/api2/index";   // GET
        public static string API_CREATE_URL = "https://simple-note.appspot.com/api2/data";   // POST
        public static string API_NOTE_URL = "https://simple-note.appspot.com/api2/data/";    // GET
        public static string API_UPDATE_URL = "https://simple-note.appspot.com/api2/data/";   // POST
        public static string API_DELETE_URL = "https://simple-note.appspot.com/api2/delete"; // POST
        public static string API_SEARCH_URL = "https://simple-note.appspot.com/api2/search"; // GET

        public static void Post(string url, string Body, Func<string, bool> fAfter)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = "POST";
            request.BeginGetRequestStream(ar =>
            {
                var requestStream = request.EndGetRequestStream(ar);
                using (var sw = new StreamWriter(requestStream))
                {
                    sw.Write(Body);
                    sw.Flush();
                    sw.Close();
                }

                request.BeginGetResponse(a =>
                {
                    try
                    {
                        var rsp = request.EndGetResponse(a);
                        var responseStream = rsp.GetResponseStream();
                        using (var sr = new StreamReader(responseStream))
                        {
                            string resp = sr.ReadToEnd();
                            sr.Close();
                            object[]  args = { resp };
                            Deployment.Current.Dispatcher.BeginInvoke(fAfter, args);
                        }
                        rsp.Close();
                    }
                    catch (Exception)
                    {
                        object[] args = { "" };
                        Deployment.Current.Dispatcher.BeginInvoke(fAfter, args);
                    }

                }, request);

            }, request);
        }

        public static void Get(string url, Func<string, bool> fAfter)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = "GET";
            request.BeginGetResponse(a =>
            {
                try
                {
                    var rsp = request.EndGetResponse(a);
                    var responseStream = rsp.GetResponseStream();
                    using (var sr = new StreamReader(responseStream))
                    {
                        string resp = sr.ReadToEnd();
                        sr.Close();
                        object[] args = { resp };
                        Deployment.Current.Dispatcher.BeginInvoke(fAfter, args);
                    }
                    rsp.Close();
                }
                catch (Exception)
                {
                    object[] args = { "" };
                    Deployment.Current.Dispatcher.BeginInvoke(fAfter, args);
                }

            }, request);
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

    }
}
