using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Globalization;

namespace ConsoleApp
{
    public class GoogleSheetsReader
    {

            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
            static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            static string ApplicationName = "Quickstart";

            public static IList<EmailObject> Read()
            {
                UserCredential credential;

                using (var stream =
                    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
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
                string spreadsheetId = "1aCp5frOgnn2K6JvnNzaR8aiYG1vf3l2NMCVcUgFBrAU";
                string range = "Events!B2:S";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);

                // Prints the names and majors of students in a sample spreadsheet:
                // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                List<EmailObject> res = new List<EmailObject>();
                if (values != null && values.Count > 0)
                {
                   
                    foreach (var row in values)
                    {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    EmailObject obj = new EmailObject()
                    {
                        id = $"{row[1].ToString().Replace(" ", string.Empty)}_{new CultureInfo(row[0].ToString())}",
                        socialShare = row[2].ToString() == "Yes",
                        eventName = row[1].ToString().Replace(" ", string.Empty),
                        subject = row[3].ToString(),
                        cultureInfo = new CultureInfo(row[0].ToString()),
                        blocks = new List<EmailBlock>()
                    };
                    if (row[4].ToString() != string.Empty)
                    {
                        obj.blocks.Add(new EmailBlock()
                        {
                            title = row[4].ToString(),
                            subtitle = row[6].ToString(),
                            body = row[7].ToString(),
                            cta = row[8].ToString(),
                            minorTitle = row[5].ToString()
                        });
                    }

                    if (row.Count > 10 && row[2].ToString() == "No")
                    {
                        obj.blocks.Add(new EmailBlock()
                        {
                            title = row[10].ToString(),
                            subtitle = row[11].ToString(),
                            body = row[12].ToString(),
                            cta = row.Count > 13 ? row[13].ToString() : string.Empty,
                        });
                    }

                    if (row.Count > 14)
                    {
                        obj.blocks.Add(new EmailBlock()
                        {
                            title = row[14].ToString(),
                            subtitle = row[15].ToString(),
                            body = row[16].ToString(),
                            cta = row[17].ToString()
                        });
                    }
                    res.Add(obj);

                    Console.WriteLine($"{row[1].ToString().Replace(" ", string.Empty)}_{new CultureInfo(row[0].ToString())}");
                    }
                return res;
                }
                else
                {
                    Console.WriteLine("No data found.");
                return null;
                }
                
            }
    }

    public class EmailObject
    {
        public string id { get; set; }
        public bool socialShare { get; set; }
        public string eventName { get; set; }

        public string subject { get; set; }

        public CultureInfo cultureInfo { get; set; }

        public IList<EmailBlock> blocks { get; set; }
    }

    public class EmailBlock
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string body { get; set; }
        public string cta { get; set; }
        public string minorTitle { get; set; }
    }

}

