using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace Test
{

    interface IGoogleSheet
    {
        Task DoWorkAsync();
    }
    internal class GoogleSheet : IGoogleSheet
    {
        private readonly IPurchaseSearch m_Search;
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";

        public GoogleSheet(IPurchaseSearch search)
        {
            m_Search = search;
        }

        public async Task DoWorkAsync()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart1.json");

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).ConfigureAwait(false);
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            var spreadsheetId = "1ngSfXV7CEFZqPZzNPie5Srjrk9V5k_ya_4em0lkIvdQ";

            var range = "schools + logos!A2:C";
            var request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            
            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            var response = await request.ExecuteAsync().ConfigureAwait(false);
            var values = response.Values;
            int i = 0;
            var change = false;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    while (row.Count < 3)
                    {
                        row.Add(null);
                    }
                    i++;
                    if (i % 10 == 0 && change)
                    {
                        await UpdateSheetAsync(values, service, spreadsheetId, range).ConfigureAwait(false);
                        change = false;
                    }
                    if (row[1] == null && row[2] == null)
                    {
                        var p = await m_Search.SearchAsync(row[0].ToString(), default).ConfigureAwait(false);

                        if (p == null)
                        {
                            
                            continue;
                        }
                        row[1] = p.Location?.Latitude;
                        row[2] = p.Location?.Longitude;
                        change = true;
                        //await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);
                    }
                   
                }
                
               
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            await UpdateSheetAsync(values, service, spreadsheetId, range).ConfigureAwait(false);

            Console.Read();
        }

        private static Task UpdateSheetAsync(IList<IList<object>> values, SheetsService service, string spreadsheetId, string range)
        {
            var body = new ValueRange {Values = values};

            var update = service.Spreadsheets.Values.Update(body, spreadsheetId, range);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            return update.ExecuteAsync();
        }
    }
}
