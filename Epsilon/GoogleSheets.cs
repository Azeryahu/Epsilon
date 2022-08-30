using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Discord.WebSocket;

namespace Epsilon
{
    class GoogleSheets
    {
        static public void StandingSheetUpdate()
        {
            var db = new DatabaseContext();
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Google Sheets API .NET Quckstart";
            UserCredential credential;
            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            // Define request parameters.
            var uploaded = db.Users.ToList();
            String spreadsheetId = "1VpgCvp6md4yt4p2wJqUGLKsa8SjeS1nB5phEgiSq2YE";
            String range = "Standings!A1";
            var dataRequest = new ValueRange();//request data
            var standingsList = new List<IList<object>>();

            standingsList.Add(new List<object>
                {
                    "Username",//item.Username,
                    "Standing", //item.Personalstanding,
                    "Dual Username",//item.DualUsername
                });
            foreach (var item in uploaded)
            {
                if (item.JoinedFaction == false && item.PersonalStanding > -5)
                {
                    standingsList.Add(new List<object>
                    {
                        item.DiscordUsername,
                        item.PersonalStanding,
                        item.DualUsername,
                    });
                }
                /*else if (item.JoinedFaction == true)
                {
                    testlist.Add(new List<object>
                    {
                        "",
                        "",
                        "",
                        "",
                    });
                }*/
            }
            dataRequest.Values = standingsList;
            var completed = service.Spreadsheets.Values.Update(dataRequest, spreadsheetId, range);
            completed.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var response2 = completed.Execute();
            Console.WriteLine($"Updated Rows: {response2.UpdatedRows.Value}");
        }
        public static string MiningSheet(string planet)
        {
            var db = new DatabaseContext();
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Google Sheets API .NET Quckstart";
            UserCredential credential;
            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            // Define request parameters.
            String spreadsheetId = "1THsXDUNvzs-zDbq0VrSeUoJCeIIkMFS_ppPSH35ZdRs";
            String range = "Mining (Alpha)";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = request.Execute();
            string ores = string.Empty;
            foreach (var row in response.Values)
            {
                if (response.Values.IndexOf(row) < 9)
                {
                    continue;
                }
                else if (row.Count > 1)
                {
                    var planetName = row[0].ToString();
                    if (planetName.Equals(planet, StringComparison.OrdinalIgnoreCase))
                    {
                        int x = 2;
                        if (!row[x].Equals(null))
                        {
                            var oreNames = response.Values[x - 1];
                            while (x < oreNames.Count - 1)
                            {
                                if (!row[x].Equals(string.Empty))
                                {
                                    ores += oreNames[x].ToString() + " " + row[x] + "\n";
                                    x++;
                                }
                                else
                                {
                                    x++;
                                }
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
            Console.WriteLine("ore request complete.");
            return ores;
        }
        public static List<string> MinecraftBlocks()
        {
            var db = new DatabaseContext();
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Google Sheets API .NET Quckstart";
            UserCredential credential;
            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            // Define request parameters.
            String spreadsheetId = "1geUEr7L1ithY93z1odbspyEqFfWuOozaycoNTqP3ElE";
            String range = "All Vanilla Blocks";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            var response = request.Execute();
            var blocks = new List<string>();
            int column = 2;
            foreach (var block in response.Values)
            {
                if (block[1].ToString().Equals("true",StringComparison.OrdinalIgnoreCase) || 
                    block[1].ToString().Equals("false",StringComparison.OrdinalIgnoreCase))
                {
                    blocks.Add(block[0].ToString());
                }
            }
            return blocks;
        }
    }
}
