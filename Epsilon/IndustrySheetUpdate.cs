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
    class IndustrySheetUpdate
    {
        static public void runUpdate()
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
            var uploaded = db.Indy.ToList();
            String spreadsheetId = "1t3QcQYAApkn1alqdgl4gVMBLki3Nv0LjywTfBFCmy5k";
            String range = "Crafting!A4";
            var dataRequest = new ValueRange();//request data
            var industryList = new List<IList<object>>();
            foreach (var item in uploaded)
            {
                if (item.Quantity > 0)
                {
                    industryList.Add(new List<object>
                    {
                        item.Username,
                        item.PartName,
                        item.Quantity,
                    });
                }
                else
                {
                    industryList.Add(new List<object>
                    {
                        "",
                        "",
                        "",
                        "",
                    });
                }
            }
            dataRequest.Values = industryList;
            var completed = service.Spreadsheets.Values.Update(dataRequest, spreadsheetId, range);
            completed.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var response2 = completed.Execute();
            Console.WriteLine($"Updated Rows: {response2.UpdatedRows.Value}");
        }
    }
}
