using HtmlAgilityPack;

namespace AthermoBase.Container
{
    public class GameListDatabaseAPI
    {
        //All Games List (shortcut)
        public class Game
        {
            public int Appid { get; set; }
            public string Name { get; set; }
        }

        public class AppList
        {
            public AppListContainer Applist { get; set; }
        }

        public class AppListContainer
        {
            public List<Game> Apps { get; set; }
        }




        // One Game Infos



        public class GameInfo
        {
            public bool Success { get; set; }
            public GameDetails Data { get; set; }
        }
        public class GameDetails
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public int Steam_AppId { get; set; }
            public string Required_Age { get; set; }
            public bool Is_Free { get; set; }
            public string Detailed_Description { get; set; }
            public string About_The_Game { get; set; }
            public string Short_Description { get; set; }
            public string Supported_Languages { get; set; }
            public string Header_Image { get; set; }
            public string Capsule_Image { get; set; }
            public string Capsule_ImageV5 { get; set; }
            public string Website { get; set; }
            public PCRequirements Pc_Requirements { get; set; }
            public MacRequirements Mac_Requirements { get; set; }
            public LinuxRequirements Linux_Requirements { get; set; }
            public string Legal_Notice { get; set; }
            public string Ext_User_Account_Notice { get; set; }
            public List<string> Developers { get; set; }
            public List<string> Publishers { get; set; }
            public PriceOverview Price_Overview { get; set; }
            public List<int> Packages { get; set; }
            public List<PackageGroup> Package_Groups { get; set; }
            public Platforms Platforms { get; set; }
            public Metacritic Metacritic { get; set; }
            public List<Category> Categories { get; set; }
            public List<Genre> Genres { get; set; }
            public List<Screenshot> Screenshots { get; set; }
            public List<Movie> Movies { get; set; }
            public Recommendations Recommendations { get; set; }
            public Achievements Achievements { get; set; }
        }

        public class PCRequirements
        {
            public string Minimum { get; set; }
            public string Recommended { get; set; }
        }

        public class MacRequirements
        {
            public string Minimum { get; set; }
            public string Recommended { get; set; }
        }

        public class LinuxRequirements
        {
            public string Minimum { get; set; }
            public string Recommended { get; set; }
        }

        public class PriceOverview
        {
            public string Currency { get; set; }
            public float Initial { get; set; }
            public float Final { get; set; }
            public int Discount_Percent { get; set; }
            public string Initial_Formatted { get; set; }
            public string Final_Formatted { get; set; }
        }

        public class PackageGroup
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string SelectionText { get; set; }
            public string SaveText { get; set; }
            public int DisplayType { get; set; }
            public bool Is_Recurring_Subscription { get; set; }
            public List<Package> Sub { get; set; }
        }

        public class Package
        {
            public int Package_Id { get; set; }
            public string Percent_Savings_Text { get; set; }
            public float Percent_Savings { get; set; }
            public string Option_Text { get; set; }
            public bool Can_Get_Free_License { get; set; }
            public bool Is_Free_License { get; set; }
            public int Price_In_Cents_With_Discount { get; set; }
        }

        public class Platforms
        {
            public bool Windows { get; set; }
            public bool Mac { get; set; }
            public bool Linux { get; set; }
        }

        public class Metacritic
        {
            public int Score { get; set; }
            public string Url { get; set; }
        }

        public class Category
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        public class Genre
        {
            public string Id { get; set; }
            public string Description { get; set; }
        }

        public class Screenshot
        {
            public int Id { get; set; }
            public string Path_Thumbnail { get; set; }
            public string Path_Full { get; set; }
        }

        public class Movie
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Thumbnail { get; set; }
            public MovieFile Webm { get; set; }
            public MovieFile Mp4 { get; set; }
            public bool Highlight { get; set; }
        }

        public class MovieFile
        {
            public string Max { get; set; }
            public string MaxUrl { get; set; }
            public string Webm480 { get; set; }
        }

        public class Recommendations
        {
            public int Total { get; set; }
        }

        public class Achievements
        {
            public int Total { get; set; }
            public List<Achievement> Highlighted { get; set; }
        }

        public class Achievement
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(input);
            return htmlDoc.DocumentNode.InnerText.Trim();
        }

        //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""Container\GamesList.mdf"";Integrated Security=True;Connect Timeout=30;";
        public GameListDatabaseAPI()
        {

        }
        /*public void GamesDatabaseTemporary()
        {
            string jsonFilePath = @"data\steamdb.json";
            var json = File.ReadAllText(jsonFilePath);
            var games = JsonConvert.DeserializeObject<List<Game>>(json);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var game in games)
                {
                    string query = "INSERT INTO Games (Sid, StoreUrl, StorePromoUrl, StoreUscore, PublishedStore, " +
                                   "PublishedMeta, PublishedStsp, PublishedHltb, PublishedIgdb, Image, Name, " +
                                   "Description, FullPrice, CurrentPrice, Discount, Platforms, Developers, " +
                                   "Publishers, Languages, Voiceovers, Categories, Genres, Tags, Achievements, " +
                                   "GfqUrl, GfqDifficulty, GfqDifficultyComment, GfqRating, GfqRatingComment, " +
                                   "GfqLength, GfqLengthComment, StspOwners, StspMdntime, HltbUrl, MetaUrl, " +
                                   "MetaScore, MetaUscore, IgdbUrl, IgdbScore, IgdbUscore, IgdbPopularity) " +
                                   "VALUES (@Sid, @StoreUrl, @StorePromoUrl, @StoreUscore, @PublishedStore, " +
                                   "@PublishedMeta, @PublishedStsp, @PublishedHltb, @PublishedIgdb, @Image, @Name, " +
                                   "@Description, @FullPrice, @CurrentPrice, @Discount, @Platforms, @Developers, " +
                                   "@Publishers, @Languages, @Voiceovers, @Categories, @Genres, @Tags, @Achievements, " +
                                   "@GfqUrl, @GfqDifficulty, @GfqDifficultyComment, @GfqRating, @GfqRatingComment, " +
                                   "@GfqLength, @GfqLengthComment, @StspOwners, @StspMdntime, @HltbUrl, @MetaUrl, " +
                                   "@MetaScore, @MetaUscore, @IgdbUrl, @IgdbScore, @IgdbUscore, @IgdbPopularity)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Sid", game.Sid);
                        command.Parameters.AddWithValue("@StoreUrl", game.StoreUrl);
                        command.Parameters.AddWithValue("@StorePromoUrl", (object)game.StorePromoUrl ?? DBNull.Value);
                        command.Parameters.AddWithValue("@StoreUscore", game.StoreUscore);
                        command.Parameters.AddWithValue("@PublishedStore", game.PublishedStore);
                        command.Parameters.AddWithValue("@PublishedMeta", game.PublishedMeta);
                        command.Parameters.AddWithValue("@PublishedStsp", game.PublishedStsp);
                        command.Parameters.AddWithValue("@PublishedHltb", game.PublishedHltb);
                        command.Parameters.AddWithValue("@PublishedIgdb", game.PublishedIgdb);
                        command.Parameters.AddWithValue("@Image", game.Image);
                        command.Parameters.AddWithValue("@Name", game.Name);
                        command.Parameters.AddWithValue("@Description", game.Description);
                        command.Parameters.AddWithValue("@FullPrice", game.FullPrice);
                        command.Parameters.AddWithValue("@CurrentPrice", game.CurrentPrice);
                        command.Parameters.AddWithValue("@Discount", (object)game.Discount ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Platforms", game.Platforms);
                        command.Parameters.AddWithValue("@Developers", game.Developers);
                        command.Parameters.AddWithValue("@Publishers", game.Publishers);
                        command.Parameters.AddWithValue("@Languages", game.Languages);
                        command.Parameters.AddWithValue("@Voiceovers", (object)game.Voiceovers ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Categories", game.Categories);
                        command.Parameters.AddWithValue("@Genres", game.Genres);
                        command.Parameters.AddWithValue("@Tags", game.Tags);
                        command.Parameters.AddWithValue("@Achievements", (object)game.Achievements ?? DBNull.Value);
                        command.Parameters.AddWithValue("@GfqUrl", game.GfqUrl);
                        command.Parameters.AddWithValue("@GfqDifficulty", game.GfqDifficulty);
                        command.Parameters.AddWithValue("@GfqDifficultyComment", (object)game.GfqDifficultyComment ?? DBNull.Value);
                        command.Parameters.AddWithValue("@GfqRating", game.GfqRating);
                        command.Parameters.AddWithValue("@GfqRatingComment", (object)game.GfqRatingComment ?? DBNull.Value);
                        command.Parameters.AddWithValue("@GfqLength", game.GfqLength);
                        command.Parameters.AddWithValue("@GfqLengthComment", (object)game.GfqLengthComment ?? DBNull.Value);
                        command.Parameters.AddWithValue("@StspOwners", game.StspOwners);
                        command.Parameters.AddWithValue("@StspMdntime", game.StspMdntime);
                        command.Parameters.AddWithValue("@HltbUrl", game.HltbUrl);
                        command.Parameters.AddWithValue("@MetaUrl", game.MetaUrl);
                        command.Parameters.AddWithValue("@MetaScore", (object)game.MetaScore ?? DBNull.Value);
                        command.Parameters.AddWithValue("@MetaUscore", (object)game.MetaUscore ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IgdbUrl", game.IgdbUrl);
                        command.Parameters.AddWithValue("@IgdbScore", (object)game.IgdbScore ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IgdbUscore", (object)game.IgdbUscore ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IgdbPopularity", (object)game.IgdbPopularity ?? DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        */
    }
}
