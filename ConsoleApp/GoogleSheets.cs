using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleApp
{
    class GoogleSheets
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API";

        public static List<KeyValuePair<string, string>> GetData(string spreadsheetId, string range)
        {
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
            using (var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName

            }))
            {

                // Define request parameters.
                //String spreadsheetId = "1G5mztkX5w9_JcbR0tQCY9_OvlszsTzh2FXuZFecAosw";
                //String range = "Subjects!B:C";   
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);


                // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit exmple for URL
                ValueRange response = request.Execute();
                var Subjectlist = new List<KeyValuePair<string, string>>();


                if (response.Values?.Count > 0)
                {
                    for (int i = 0; i < response.Values.Count; i++)
                    {
                        if (response.Values[i].Count == 2)
                        {
                            Subjectlist.Add(new KeyValuePair<string, string>(response.Values[i][0].ToString(), response.Values[i][1].ToString()));
                        }
                        else
                        {
                            Subjectlist.Add(new KeyValuePair<string, string>(response.Values[i][0].ToString(), ""));
                        }
                    }

                }
                else
                {
                    Console.WriteLine("No data found.");
                }

                //if (response.Values?.Count > 0)
                //{
                //   // Console.WriteLine("Subject, Topic");
                //    foreach (var row in response.Values)
                //    {

                //        if (row.Count==2) {
                //            Console.WriteLine(row[0] + " " + row[1]);
                //            continue;
                //        }
                //        if (row.Count==1)
                //        { Console.WriteLine(row[0]); }
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("No data found.");
                //}
                //Console.Read();
                return Subjectlist;
            }
        }
    }
}
