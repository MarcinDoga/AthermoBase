using Newtonsoft.Json;

namespace AthermoBase.Container
{
    public class SettingsAPI
    {
        private Dictionary<string, string> settings;
        string settingsPath;
        public SettingsAPI(string settingsPath)
        {
            this.settingsPath = settingsPath;
            string jsonText = System.IO.File.ReadAllText(settingsPath);
            this.settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
        }
        public string LoadSetting(string key)
        {
            if (this.settings.ContainsKey(key))
            {
                return this.settings[key];
            }
            else
            {
                return null;
            }
        }
        public void SetSetting(string key, string value)
        {
            this.settings[key] = value;
            string json = JsonConvert.SerializeObject(this.settings, Formatting.Indented);
            System.IO.File.WriteAllText(settingsPath, json);
        }
    }
}
