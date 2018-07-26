using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cloudents.Core.Command;

namespace ConsoleApp
{
    internal class GoogleSheets
    {
        static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private const string ApplicationName = "Google Sheets API";

        public static List<CreateQuestionCommand> GetData(string spreadsheetId, string range)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
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
                var request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);

                var response = request.Execute();
                var subjectList = new List<CreateQuestionCommand>();

                if (response.Values?.Count > 0)
                {
                    for (int i = 0; i < response.Values.Count; i++)
                    {
                        if (response.Values[i].Count == 3)
                        {
                            CreateQuestionCommand t = new CreateQuestionCommand();
                            t.SubjectId = Convert.ToInt32(response.Values[i][2]);
                            t.Text = response.Values[i][0].ToString();
                            t.Price = Convert.ToDecimal(response.Values[i][1]);
                            t.UserId = 1;
                            subjectList.Add(t);
                        }
                       /* if (response.Values[i].Count == 2)
                        {
                            subjectList.Add(new KeyValuePair<string, string>(response.Values[i][0].ToString(), response.Values[i][1].ToString()));
                        }
                        else
                        {
                            subjectList.Add(new KeyValuePair<string, string>(response.Values[i][0].ToString(), ""));
                        }*/
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
                return subjectList;
            }
        }
    }
}
