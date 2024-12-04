using Microsoft.Win32;
using System.Management;

namespace AthermoBase.Container
{
    public class WindowsElementsAPI
    {
        public WindowsElementsAPI() { }
        public List<string> styleMode(bool windowsMode)
        {
            List<string> styles = new List<string>();
            if (windowsMode)
            {
                styles.Add("windowsMode");
            }
            string mode = "Dark";
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key != null)
                {
                    var value = key.GetValue("AppsUseLightTheme");
                    if (value != null)
                    {
                        mode = (int)value == 0 ? "Dark" : "Light";
                    }
                }
            }
            catch (Exception)
            {
                mode = "ERROR";
            }
            styles.Add(mode);
            return styles;
        }
        public string coresAndRam()
        {
            int cores = 0;
            int ram = 0;
            try
            {
                cores = Environment.ProcessorCount;

                var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                foreach (var item in searcher.Get())
                {
                    ram = Convert.ToInt32(Math.Round(Convert.ToDouble(item["TotalVisibleMemorySize"]) / 1024 / 1024, 1));
                }
            }
            catch (Exception ex)
            {
                cores = 0;
                ram = 0;
            }
            return $"{cores}|{ram}";
        }
    }
}
