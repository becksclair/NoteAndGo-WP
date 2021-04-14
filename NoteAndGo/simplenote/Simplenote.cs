using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using NoteAndGo.Data;
using NoteAndGo.Helpers;

namespace NoteAndGo.simplenote
{
    public class Simplenote
    {
        private string Username;
        private string Password;
        private string Token;
        private ViewModelNotes ViewModel;

        public Simplenote(ViewModelNotes vm)
        {
            ViewModel = vm;
            GlobalLoading.Instance.SetLoading(true, "Loading...");

            AppSettings settings = new AppSettings();
            Username = settings.GetValueOrDefault("SNUsername", "");
            Password = settings.GetValueOrDefault("SNPassword", "");

            if (!Utils.InternetIsAvailable()) return;
            GlobalLoading.Instance.SetLoading(true, "Loading...");
            Login(GetIndex);
        }

        private void Login(Func<bool> fAfter)
        {
            var authbody = WebHelper.EncodeTo64("email=" + Username + "&password=" + Password);
            WebHelper.Post(WebHelper.API_LOGIN_URL, authbody, resp =>
            {
                if (resp == "")
                {
                    MessageBox.Show("Wrong username or password");
                    return false;
                }
                Token = resp;

                if (fAfter != null)
                    fAfter();
                return true;
            });
        }

        private bool GetIndex()
        {
            return GetIndex(null);
        }

        private bool GetIndex(string mark)
        {
            var url = WebHelper.API_NOTES_URL + "?length=30&auth=" + Token + "&email=" + Username;

            if (mark != null)
            {
                url = url + "&mark=" + mark;
            }

            WebHelper.Get(url, resp =>
            {
                Sync(resp);
                return true;
            });
            return true;
        }

        private void Sync(string Index)
        {
            if (Index == "")
            {
                GlobalLoading.Instance.IsLoading = false;
                return;
            }

            JObject o = JObject.Parse(Index);
            string mark = (string)o["mark"];
            JArray notes = (JArray) o["data"];

            foreach (var note in ViewModel.Items)
            {
                if (note.Key == "" && note.Changed == true)
                {
                    CreateRemote(note);
                }
                else if (note.Key != "" && note.Changed == true)
                {
                    UpdateRemote(note);
                }
            }

            Collection<Note> tempCollection = new Collection<Note>();
            foreach (var note in notes)
            {
                var key = (string)note["key"];
                var version = (int)note["version"];
                var syncnum = (int)note["syncnum"];
                var deleted = (long)note["deleted"];
                Note PrevNote = ViewModel.FindByKey(key);

                if (PrevNote != null)
                {
                    if (deleted == 1)
                    {
                        ViewModel.DeleteNote(PrevNote);
                        continue;
                    }

                    // Server is newer
                    if (PrevNote.Syncnum < syncnum || PrevNote.Version < version)
                    {
                        UpdateLocal(PrevNote);
                    }
                }
                else
                {
                    if (deleted == 0)
                    {
                        Note NewNote = new Note { Key = key };
                        CreateLocal(NewNote);
                    }
                }
            }

            if (mark != null)
            {
                GetIndex(mark);
                return;
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        private void CreateLocal(Note Note)
        {
            var url = WebHelper.API_NOTE_URL + Note.Key + "?auth=" + Token + "&email=" + Username;
            WebHelper.Get(url, resp =>
            {
                JObject o = JObject.Parse(resp);
                Note.Content = (string)o["content"];
                Note.Syncnum = (int)o["syncnum"];
                Note.Key = (string)o["key"];
                Note.ModifyDate = (string)o["modifydate"];
                Note.CreateDate = (string)o["createdate"];
                Note.Deleted = (long)o["deleted"];
                Note.Version = (int)o["version"];
                JArray tagsArray = (JArray)o["tags"];
                foreach (var tag in tagsArray)
                {
                    Note.Tags = (string)tag;
                }

                ViewModel.InsertNote(Note);
                return true;
            });
        }

        private void UpdateLocal(Note Note)
        {
            var url = WebHelper.API_NOTE_URL + Note.Key + "?auth=" + Token + "&email=" + Username;
            WebHelper.Get(url, resp =>
            {
                JObject o = JObject.Parse(resp);
                Note.Content = (string)o["content"];
                Note.Syncnum = (int)o["syncnum"];
                Note.Key = (string)o["key"];
                Note.ModifyDate = (string)o["modifydate"];
                Note.CreateDate = (string)o["createdate"];
                Note.Deleted = (long)o["deleted"];
                Note.Version = (int)o["version"];
                JArray tagsArray = (JArray)o["tags"];
                foreach (var tag in tagsArray)
                {
                    Note.Tags = (string)tag;
                }

                ViewModel.SaveChanges();
                return true;
            });
        }

        private void CreateRemote(Note Note)
        {
            var json = new JObject();
            json.Add("content", Note.Content);
            JArray tagsArray = new JArray();
            foreach (var tag in Note.Tags.Split(','))
            {
                tagsArray.Add(tag);
            }
            json.Add(tagsArray);

            var url = WebHelper.API_CREATE_URL + "?auth=" + Token + "&email=" + Username;
            WebHelper.Post(url, json.ToString(), resp =>
            {
                JObject o = JObject.Parse(resp);
                Note.Tags = "";
                Note.Syncnum = (int)o["syncnum"];
                Note.Key = (string)o["key"];
                Note.ModifyDate = (string)o["modifydate"];
                Note.CreateDate = (string)o["createdate"];
                Note.Deleted = (long)o["deleted"];
                Note.Version = (int)o["version"];

                ViewModel.SaveChanges();
                return true;
            });
        }

        public void UpdateRemote(Note Note)
        {
            var json = new JObject();
            json.Add("content", Note.Content);
            if (Note.Deleted == 1)
                json.Add("deleted", Note.Deleted);
            json.Add("modifydate", Note.ModifyDate);
            json.Add("version", Note.Version);
            JArray tagsArray = new JArray();
            foreach (var tag in Note.Tags.Split(','))
            {
                tagsArray.Add(tag);
            }
            json.Add(tagsArray);

            var url = WebHelper.API_UPDATE_URL + Note.Key + "?auth=" + Token + "&email=" + Username;
            WebHelper.Post(url, json.ToString(), resp =>
            {
                if (resp == "") return false;

                JObject o = JObject.Parse(resp);
                Note.Syncnum = (int)o["syncnum"];
                Note.ModifyDate = (string)o["modifydate"];
                Note.Version = (int)o["version"];

                ViewModel.SaveChanges();
                return true;
            });
        }
    }
}
