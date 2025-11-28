using System;
using System.IO;
using Newtonsoft.Json;

namespace QLTN_LT.BLL
{
    public class UserPreferences
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_prefs.json");

        public string Username { get; set; }
        public bool RememberMe { get; set; }

        public static void Save(UserPreferences prefs)
        {
            try
            {
                string json = JsonConvert.SerializeObject(prefs, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception)
            {
                // Handle or log error
            }
        }

        public static UserPreferences Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    return JsonConvert.DeserializeObject<UserPreferences>(json) ?? new UserPreferences();
                }
            }
            catch (Exception)
            {
                // Handle or log error
            }
            return new UserPreferences();
        }
    }
}
