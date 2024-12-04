using Newtonsoft.Json;
using Octokit;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace AthermoBase.Container
{
    class PlayerAPI
    {

        public string userFile = string.Empty;

        public async Task<string> InitializeAccountAsync(string playerID)
        {
            HttpClient client = new HttpClient();
            try
            {
                userFile = await client.GetStringAsync("https://raw.githubusercontent.com/MarcinDoga/Users/Users/" + playerID);
                return userFile;
            }
            catch (HttpRequestException e)
            {
                return "none";
            }
        }
        public Dictionary<string, (Color, int)> ranksAndColors = new Dictionary<string, (Color, int)>
{
    { "Default", (Colors.Gray, 13) },
    { "VIP", (Colors.DarkGreen, 12) },
    { "VIP+", (Colors.Green, 11) },
    { "MVP", (Colors.BlueViolet, 10) },
    { "MVP+", (Colors.DarkSlateBlue, 9) },
    { "Moderator", (Colors.Blue, 8) },
    { "Support", (Colors.YellowGreen, 7) },
    { "Pro Player", (Colors.Orange, 6) },
    { "Epic Gamer", (Colors.DarkOrange, 5) },
    { "Legend", (Colors.OrangeRed, 4) },
    { "Champion", (Colors.MediumVioletRed, 3) },
    { "Admin", (Colors.DarkRed, 2) },
    { "Owner", (Colors.DarkViolet, 1) }
};
        public readonly Dictionary<string, float> rankMultipliers = new Dictionary<string, float>
    {
        { "VIP", 1.5f },
        { "VIP+", 2.0f },
        { "MVP", 2.5f },
        { "MVP+", 3.0f }
    };
        private static byte[] key = Encoding.UTF8.GetBytes("key");
        private static byte[] iv = Encoding.UTF8.GetBytes("key");
        public string EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public string DecryptString(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        public string LoadPlayerFile(string key, string path)
        {
            string jsonText = System.IO.File.ReadAllText(path);
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);

            if (data.ContainsKey(key))
            {
                return data[key];
            }
            else
            {
                return null;
            }
        }
        public void SetPlayerFile(string key, string value, string path)
        {
            string jsonText = System.IO.File.ReadAllText(path);
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);

            data[key] = value;
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            System.IO.File.WriteAllText(path, json);
        }

        public string CreatePlayer(TimeSpan startInSecondsTime, DateTime startAppTime, string playerID, string nickname, string email)
        {
            ManagementObjectSearcher searcher4 = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");
            string graphicsCard = "";
            foreach (ManagementObject mo in searcher4.Get())

                foreach (PropertyData property4 in mo.Properties)
                {
                    if (property4.Name == "Description")
                    {
                        graphicsCard += property4.Value.ToString();
                    }
                }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ManagementObjectCollection collection = searcher.Get();
            string procName = null;
            foreach (ManagementObject obj in collection)
            {
                string processorName = obj["Name"].ToString();
                procName = processorName;
            }
            string result = string.Empty;
            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher2.Get())
            {
                result = os["Caption"].ToString();
            }
            CultureInfo culture = CultureInfo.CurrentCulture;
            string systemLanguage = culture.DisplayName;
            var gitHubClient = new GitHubClient(new Octokit.ProductHeaderValue("Users"));
            gitHubClient.Credentials = new Credentials("gpg");
            var user = gitHubClient.User.Get("MarcinDoga");
            string userName = Environment.UserName;
            FileInfo file = new FileInfo("AthermoBase.exe");
            DateTime dt = file.CreationTime;

            Dictionary<string, string> player = new Dictionary<string, string>();
            player["userName"] = userName;
            player["userId"] = playerID;
            player["nickName"] = nickname;
            player["email"] = email;
            player["level"] = "0";
            player["ranks"] = "Default";
            player["lastPlayedGame"] = "AthermoBase";
            player["lastOnline"] = DateTime.Now.ToString("dd.MM.yyyy HH.mm");
            player["lastReward"] = DateTime.Now.ToString("dd.MM.yyyy HH.mm");
            player["createAccountTime"] = DateTime.Now.ToString("dd.MM.yyyy HH.mm");
            player["startTime"] = startInSecondsTime.ToString();
            player["userLanguage"] = systemLanguage;
            player["windowsVersion"] = result;
            player["processor"] = procName.TrimEnd();
            player["graphic"] = graphicsCard;
            player["installTime"] = dt.ToString("dd.MM.yyyy HH.mm");
            string json = JsonConvert.SerializeObject(player, Formatting.Indented);
            var (owner, repoName, filePath, branch) = ("MarcinDoga", "Users",
            playerID, "Users");
            try
            {
                gitHubClient.Repository.Content.CreateFile(
                     owner, repoName, filePath,
                     new CreateFileRequest($"{filePath}", json, branch));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Error while Creating Player Account!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            return json;
        }
        public string UpdatePlayer(TimeSpan startInSecondsTime, DateTime startAppTime, string playerID, string nickname, string email, float lvl, string ranks, DateTime lastReward, string lastPlayedGame, DateTime accountCreationTime)
        {
            ManagementObjectSearcher searcher4 = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");
            string graphicsCard = "";
            foreach (ManagementObject mo in searcher4.Get())

                foreach (PropertyData property4 in mo.Properties)
                {
                    if (property4.Name == "Description")
                    {
                        graphicsCard += property4.Value.ToString();
                    }
                }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ManagementObjectCollection collection = searcher.Get();
            string procName = null;
            foreach (ManagementObject obj in collection)
            {
                string processorName = obj["Name"].ToString();
                procName = processorName;
            }
            string result = string.Empty;
            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher2.Get())
            {
                result = os["Caption"].ToString();
            }
            CultureInfo culture = CultureInfo.CurrentCulture;
            string systemLanguage = culture.DisplayName;
            var gitHubClient = new GitHubClient(new Octokit.ProductHeaderValue("Users"));
            gitHubClient.Credentials = new Credentials("gpg");
            var user = gitHubClient.User.Get("MarcinDoga");
            string userName = Environment.UserName;
            FileInfo file = new FileInfo("AthermoBase.exe");
            DateTime dt = file.CreationTime;

            Dictionary<string, string> player = new Dictionary<string, string>();
            player["userName"] = userName;
            player["userId"] = playerID;
            player["nickName"] = nickname;
            player["email"] = email;
            player["level"] = lvl.ToString();
            player["ranks"] = ranks;
            player["lastPlayedGame"] = lastPlayedGame;
            player["lastOnline"] = DateTime.Now.ToString("dd.MM.yyyy HH.mm");
            player["lastReward"] = lastReward.ToString("dd.MM.yyyy HH.mm");
            player["createAccountTime"] = accountCreationTime.ToString("dd.MM.yyyy HH.mm");
            player["startTime"] = startInSecondsTime.ToString();
            player["userLanguage"] = systemLanguage;
            player["windowsVersion"] = result;
            player["processor"] = procName.TrimEnd();
            player["graphic"] = graphicsCard;
            player["installTime"] = dt.ToString("dd.MM.yyyy HH.mm");
            string json = JsonConvert.SerializeObject(player, Formatting.Indented);
            var (owner, repoName, filePath, branch) = ("MarcinDoga", "Users",
            playerID, "Users");
            try
            {
                var readfile = gitHubClient.Repository.Content.GetAllContentsByRef(owner, repoName, filePath, branch).Result;
                var updatefile = gitHubClient.Repository.Content.UpdateFile(owner, repoName, filePath,
                new UpdateFileRequest($"{filePath}", json, readfile.First().Sha, branch));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Error while Updating Player Account!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            return json;
        }
    }
}