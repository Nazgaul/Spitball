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
    public class MigrateCoursesAndUni
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets};
        static string ApplicationName = "Quickstart";

        public static IList<CorseToMigratre> Read()
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
            string spreadsheetId = "15HTW3DY9zpSubj6sDnCiF74sMe4kbZ9ksxlREEU4u6Q";
            string range = "Courses!A2:D";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            List<CorseToMigratre> res = new List<CorseToMigratre>();
            if (values != null && values.Count > 0)
            {

                foreach (var row in values)
                {
                    long.TryParse(row[0].ToString(), out var t);
                    // Print columns A and E, which correspond to indices 0 and 4.
                    CorseToMigratre obj = new CorseToMigratre()
                    {
                        
                        Id = t,
                        Name = row[1].ToString(),
                        NewId = row[3].ToString()
                    };
                   
                    res.Add(obj);
                }
                return res;
            }
            else
            {
                Console.WriteLine("No data found.");
                return null;
            }

        }

        public static void WriteToSheet(int rowNumber)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
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



            String spreadsheetId2 = "15HTW3DY9zpSubj6sDnCiF74sMe4kbZ9ksxlREEU4u6Q";
            String range2 = $"Courses!E{rowNumber}";  // update cell F5 
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

            var oblist = new List<object>() { "Not Processed" };
            valueRange.Values = new List<IList<object>> { oblist };

            SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId2, range2);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result2 = update.Execute();

        }



        public static IList<UniversityToMigratre> ReadUniversity()
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
            string spreadsheetId = "15HTW3DY9zpSubj6sDnCiF74sMe4kbZ9ksxlREEU4u6Q";
            string range = "Uni ISL!A2:E";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            List<UniversityToMigratre> res = new List<UniversityToMigratre>();
            if (values != null && values.Count > 0)
            {

                foreach (var row in values)
                {
                    Guid.TryParse(row[0].ToString(), out var uniid);
                    int.TryParse(row[1].ToString(), out var oldid);
                    // Print columns A and E, which correspond to indices 0 and 4.
                    UniversityToMigratre obj = new UniversityToMigratre()
                    {
                        UniId = uniid,
                        OldId = oldid,
                        Name = row[2].ToString(),
                        NewId = row[4].ToString()
                    };

                    res.Add(obj);
                }
                return res;
            }
            else
            {
                Console.WriteLine("No data found.");
                return null;
            }

        }


        public static void WriteToUniSheet(int rowNumber)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
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



            String spreadsheetId2 = "15HTW3DY9zpSubj6sDnCiF74sMe4kbZ9ksxlREEU4u6Q";
            String range2 = $"Uni ISL!J{rowNumber}";  // update cell F5 
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

            var oblist = new List<object>() { "Not Processed" };
            valueRange.Values = new List<IList<object>> { oblist };

            SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId2, range2);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result2 = update.Execute();

        }

    }

    public class CorseToMigratre
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NewId { get; set; }
    }

    public class UniversityToMigratre
    {
        public Guid UniId { get; set; }
        public int OldId { get; set; }
        public string Name { get; set; }
        public string NewId { get; set; }
    }
}
