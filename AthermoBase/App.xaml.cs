using AthermoBase.Container;
using Newtonsoft.Json;
using Octokit;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using Formatting = Newtonsoft.Json.Formatting;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AthermoBase
{
    public partial class App : Application
    {
        private static Mutex _mutex;

        private static bool initialized = false;
        private static int firstStart = 0;
        private static string athermoBaseStartupName = "AthermoBase Thread 0: OnStartup ";
        private static string owner = "MarcinDoga";
        private static string repo = "AthermoBase";


        protected override void OnStartup(StartupEventArgs e)
        {
            string appGXName = "AthermoBase";
            bool isNewInstance;
            _mutex = new Mutex(true, appGXName, out isNewInstance);

            if (!isNewInstance)
            {
                IntPtr handle = FindWindow(null, "AthermoBase");
                if (handle != IntPtr.Zero)
                {
                    MessageBox.Show("The application is already running, possible access through the notifications icon.", athermoBaseStartupName + "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Environment.Exit(0);
                return;
            }

            LanguageAPI languageAPI = new LanguageAPI();
            CheckConnection();
            if (!Directory.Exists(@"data/"))
            {
                firstStart = 1;
                Directory.CreateDirectory(@"data/");
            }
            if (!Directory.Exists(@"data\images\Background"))
            {
                firstStart = 1;
                Directory.CreateDirectory(@"data\images\Background");
            }
            if (!Directory.Exists(@"data\Lang"))
            {
                Directory.CreateDirectory(@"data\Lang");
            }
            if (!Directory.Exists(@"data\Lang\Hash"))
            {
                Directory.CreateDirectory(@"data\Lang\Hash");
            }
            CultureInfo culture = CultureInfo.CurrentCulture;
            string systemCountry = culture.Name;
            string[] drives = Directory.GetLogicalDrives();
            List<string> folders = new List<string>();

            try
            {
                string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                // Steam
                string steamPath = Path.Combine(programFilesX86, "Steam", "steamapps", "common");
                if (Directory.Exists(steamPath))
                {
                    folders.Add(steamPath);
                }
                // Epic Games
                string epicGamesPath = Path.Combine(programFiles, "Epic Games");
                if (Directory.Exists(epicGamesPath))
                {
                    folders.Add(epicGamesPath);
                }
                // Origin
                string originGamesPath = Path.Combine(programFilesX86, "Origin Games");
                if (Directory.Exists(originGamesPath))
                {
                    folders.Add(originGamesPath);
                }
                // Ubisoft
                string ubisoftPath = Path.Combine(programFilesX86, "Ubisoft", "Ubisoft Game Launcher", "games");
                if (Directory.Exists(ubisoftPath))
                {
                    folders.Add(ubisoftPath);
                }
                // GOG Galaxy
                string gogGalaxyPath = Path.Combine(programFilesX86, "GOG Galaxy", "Games");
                if (Directory.Exists(gogGalaxyPath))
                {
                    folders.Add(gogGalaxyPath);
                }
                foreach (string drive in drives)
                {
                    string gamesPath = Path.Combine(drive, "Games");
                    if (Directory.Exists(gamesPath))
                    {
                        folders.Add(gamesPath);
                    }
                    string steamLibraryPath = Path.Combine(drive, "SteamLibrary");
                    if (Directory.Exists(steamLibraryPath))
                    {
                        folders.Add(steamLibraryPath);
                    }
                    string steam = Path.Combine(drive, "Steam");
                    if (Directory.Exists(steam))
                    {
                        folders.Add(steam);
                    }
                    string epic = Path.Combine(drive, "Epic Games");
                    if (Directory.Exists(epic))
                    {
                        folders.Add(epic);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            string settingsPath = @"data\ABSettings.json";
            string messagesPath = @"data\ABSavedata.json";
            if (!File.Exists(messagesPath))
            {
                Dictionary<string, string> playerData = new Dictionary<string, string>();
                playerData["LastAutoSave"] = DateTime.Now.ToString();
                playerData["LastUpdate"] = DateTime.Now.ToString();
                playerData["PlayerJson"] = "none";
                if (!System.IO.File.Exists(messagesPath))
                {
                    string json = JsonConvert.SerializeObject(playerData, Formatting.Indented);
                    System.IO.File.WriteAllText(messagesPath, json);
                }

            }



            Dictionary<string, string> settings = new Dictionary<string, string>();

            if (!File.Exists(settingsPath))
            {
                WindowsElementsAPI windowsElements = new WindowsElementsAPI();
                if (languageAPI.languages.Contains(systemCountry))
                {
                    settings["language"] = systemCountry;
                }
                else
                {
                    settings["language"] = "en-US";
                }
                settings["scanDirectories"] = $"{string.Join("|", folders)}";
                settings["autochangelog"] = "true";
                settings["developerSettings"] = "false";
                settings["musicEffects"] = "true";
                settings["autoUpdate"] = "true";
                settings["updateMode"] = "stable";
                settings["mode"] = windowsElements.styleMode(true)[0];
                settings["collection"] = "NONE";
                settings["lastGamesListUpdate"] = DateTime.Now.ToString();
                settings["backAnimationSecondsTime"] = "7";
                settings["autoSaveRefreshTime"] = "5";
                settings["multiplier"] = "0";
                settings["privacy"] = "0";
                if (!System.IO.File.Exists(settingsPath))
                {
                    string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                    System.IO.File.WriteAllText(settingsPath, json);
                }
            }
            SettingsAPI settingsAPI = new SettingsAPI(settingsPath);
            if (settingsAPI.LoadSetting("autoUpdate") == "true")
            {
                string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                _ = GithubReleases(currentVersion, GetReleaseType(settingsAPI.LoadSetting("updateMode")));
            }
        }
        public async Task GithubReleases(string currentVersion, string updateMode)
        {
            var client = new GitHubClient(new ProductHeaderValue("AthermoBase-VersionChecker"));

            var latestRelease = await GetLatestReleaseAsync(client, owner, repo);
            if (latestRelease != null)
            {
                string releaseType = GetReleaseType(latestRelease.TagName);

                string latestVersion = Regex.Match(latestRelease.TagName, @"\d+(\.\d+){3}").Value;
                //MessageBox.Show($"Your version {currentVersion} ({updateMode})\nLatest version {latestVersion} ({releaseType})");

                //Porównanie wersji :D
                if (CompareVersions(latestVersion, currentVersion) > 0 && releaseType == updateMode)
                {
                    //MessageBox.Show($"Latest version {latestVersion} ({releaseType}) is available.");
                    //Pamiętaj, linkacz do pobrania!!: latestRelease.HtmlUrl
                }
            }
            else
            {
                MessageBox.Show("Error downloading latest update", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static async Task<Release?> GetLatestReleaseAsync(GitHubClient client, string owner, string repo)
        {
            try
            {
                var releases = await client.Repository.Release.GetAll(owner, repo);
                return releases.Count > 0 ? releases[0] : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }
        public static int CompareVersions(string currentVersion, string githubVersion)
        {
            Version current = new(currentVersion);
            Version github = new(githubVersion);

            return current.CompareTo(github);
        }
        private static string GetReleaseType(string tagName)
        {
            if (tagName.Contains("alpha", StringComparison.OrdinalIgnoreCase))
            {
                return "Alpha";
            }
            else if (tagName.Contains("beta", StringComparison.OrdinalIgnoreCase))
            {
                return "Beta";
            }
            else
            {
                return "Stable";
            }
        }
        private static async void CheckConnection()
        {
            while (true)
            {
                try
                {
                    using (var client = new HttpClient())
                        await client.GetAsync("http://google.com/generate_204");
                }
                catch
                {
                    MessageBox.Show("You don't have an internet connection!", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            base.OnExit(e);
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
