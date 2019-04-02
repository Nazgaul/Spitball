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
            static string[] _scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            static string _applicationName = "Quickstart";

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
                        _scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Google Sheets API service.
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = _applicationName,
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
                        Id = $"{row[1].ToString().Replace(" ", string.Empty)}_{new CultureInfo(row[0].ToString())}",
                        SocialShare = row[2].ToString() == "Yes",
                        EventName = row[1].ToString().Replace(" ", string.Empty),
                        Subject = row[3].ToString(),
                        CultureInfo = new CultureInfo(row[0].ToString()),
                        Blocks = new List<EmailBlock>()
                    };
                    if (row[4].ToString() != string.Empty)
                    {
                        obj.Blocks.Add(new EmailBlock()
                        {
                            Title = row[4].ToString(),
                            Subtitle = row[6].ToString(),
                            Body = row[7].ToString(),
                            Cta = row[8].ToString(),
                            MinorTitle = row[5].ToString()
                        });
                    }

                    if (row.Count > 10 && row[2].ToString() == "No")
                    {
                        obj.Blocks.Add(new EmailBlock()
                        {
                            Title = row[10].ToString(),
                            Subtitle = row[11].ToString(),
                            Body = row[12].ToString(),
                            Cta = row.Count > 13 ? row[13].ToString() : string.Empty,
                        });
                    }

                    if (row.Count > 14)
                    {
                        obj.Blocks.Add(new EmailBlock()
                        {
                            Title = row[14].ToString(),
                            Subtitle = row[15].ToString(),
                            Body = row[16].ToString(),
                            Cta = row[17].ToString()
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
        public string Id { get; set; }
        public bool SocialShare { get; set; }
        public string EventName { get; set; }

        public string Subject { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public IList<EmailBlock> Blocks { get; set; }
    }

    public class EmailBlock
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public string Cta { get; set; }
        public string MinorTitle { get; set; }
    }

}

