using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;

namespace NoteAndGo
{
    public class AppSettings
    {
        // Our isolated storage settings
        IsolatedStorageSettings settings;

        // The isolated storage key names of our settings
        const string SNEnabledKeyName = "SNEnabled";
        const string SNUsernameKeyName = "SNUsername";
        const string SNPasswordKeyName = "SNPassword";
        const string RateButtonUsedKeyName = "RateButtonUsed";

        // Default settings values
        const bool SNEnabledDefault = false;
        const string SNUsernameDefault = "";
        const string SNPasswordDefault = "";
        const bool RateButtonUsedDefault = false;

        /// <summary
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            try
            {
                // Get the settings for this application.
                settings = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while using IsolatedStorageSettings: " + e.ToString());
            }
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }

        /// <summary>
        /// Property to get and set a CheckBox Setting Key.
        /// </summary>
        public bool SNEnabled
        {
            get
            {
                return GetValueOrDefault<bool>(SNEnabledKeyName, SNEnabledDefault);
            }
            set
            {
                if (AddOrUpdateValue(SNEnabledKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string SNUsername
        {
            get
            {
                return GetValueOrDefault<string>(SNUsernameKeyName, SNUsernameDefault);
            }
            set
            {
                if (AddOrUpdateValue(SNUsernameKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string SNPassword
        {
            get
            {
                return GetValueOrDefault<string>(SNPasswordKeyName, SNPasswordDefault);
            }
            set
            {
                if (AddOrUpdateValue(SNPasswordKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a CheckBox Setting Key.
        /// </summary>
        public bool RateButtonUsed
        {
            get
            {
                return GetValueOrDefault<bool>(RateButtonUsedKeyName, RateButtonUsedDefault);
            }
            set
            {
                if (AddOrUpdateValue(RateButtonUsedKeyName, value))
                {
                    Save();
                }
            }
        }
    }
}
