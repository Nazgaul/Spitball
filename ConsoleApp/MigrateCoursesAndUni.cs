//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Sheets.v4;
//using Google.Apis.Sheets.v4.Data;
//using Google.Apis.Services;
//using Google.Apis.Util.Store;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;

//namespace ConsoleApp
//{
//    public class MigrateCoursesAndUni
//    {
//        static string[] _scopes = { SheetsService.Scope.Spreadsheets};
//        static readonly string _applicationName = "Quickstart";

//        public static IList<CorseToMigratre> Read()
//        {
//            UserCredential credential;

//            using (var stream =
//                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
//            {
//                // The file token.json stores the user's access and refresh tokens, and is created
//                // automatically when the authorization flow completes for the first time.
//                var credPath = "token.json";
//                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.Load(stream).Secrets,
//                    _scopes,
//                    "user",
//                    CancellationToken.None,
//                    new FileDataStore(credPath, true)).Result;
//                Console.WriteLine("Credential file saved to: " + credPath);
//            }

//            // Create Google Sheets API service.
//            var service = new SheetsService(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = _applicationName,
//            });

//            // Define request parameters.
//            string spreadsheetId = "1O23NzfssFjz2lKLxY4HBQBP6O_B0U0n42jLx6hrRQXY";
//            string range = "Sheet8!A2:B";
//            SpreadsheetsResource.ValuesResource.GetRequest request =
//                    service.Spreadsheets.Values.Get(spreadsheetId, range);

//            // Prints the names and majors of students in a sample spreadsheet:
//            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
//            ValueRange response = request.Execute();
//            IList<IList<Object>> values = response.Values;
//            List<CorseToMigratre> res = new List<CorseToMigratre>();
//            if (values != null && values.Count > 0)
//            {

//                foreach (var row in values)
//                {
//                    //long.TryParse(row[1].ToString(), out var t);
//                    // Print columns A and E, which correspond to indices 0 and 4.
//                    CorseToMigratre obj = new CorseToMigratre()
//                    {
//                        NewName = row[0].ToString(),
//                        OldName = row[1].ToString(),
//                        Action = row[2].ToString(),
//                        NewId = row[3].ToString()
//                    };

//                    res.Add(obj);
//                }
//                return res;
//            }
//            else
//            {
//                Console.WriteLine("No data found.");
//                return null;
//            }

//        }

//        public static void WriteToSheet(int rowNumber)
//        {
//            UserCredential credential;

//            using (var stream =
//                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
//            {
//                string credPath = "token.json";


//                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.Load(stream).Secrets,
//                    _scopes,
//                    "user",
//                    CancellationToken.None,
//                    new FileDataStore(credPath, true)).Result;
//                Console.WriteLine("Credential file saved to: " + credPath);
//            }

//            // Create Google Sheets API service.
//            var service = new SheetsService(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = _applicationName,
//            });


//            String spreadsheetId2 = "1O23NzfssFjz2lKLxY4HBQBP6O_B0U0n42jLx6hrRQXY";
//            String range2 = $"Sheet8!C{rowNumber}";  // update cell F5 
//            ValueRange valueRange = new ValueRange();
//            valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

//            var oblist = new List<object>() { "Not Processed" };
//            valueRange.Values = new List<IList<object>> { oblist };

//            SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId2, range2);
//            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
//            UpdateValuesResponse result2 = update.Execute();

//        }



//        public static IList<UniversityToMigratre> ReadUniversity()
//        {
//            UserCredential credential;

//            using (var stream =
//                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
//            {
//                // The file token.json stores the user's access and refresh tokens, and is created
//                // automatically when the authorization flow completes for the first time.
//                string credPath = "token.json";
//                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.Load(stream).Secrets,
//                    _scopes,
//                    "user",
//                    CancellationToken.None,
//                    new FileDataStore(credPath, true)).Result;
//                Console.WriteLine("Credential file saved to: " + credPath);
//            }

//            // Create Google Sheets API service.
//            var service = new SheetsService(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = _applicationName,
//            });

//            // Define request parameters.
//            string spreadsheetId = "1gkK8vARdUhwtzVGthpFdp0BZOhHY2KfZTpvN0oU94l8";
//            string range = "Sheet1!A2:D";
//            SpreadsheetsResource.ValuesResource.GetRequest request =
//                    service.Spreadsheets.Values.Get(spreadsheetId, range);

//            // Prints the names and majors of students in a sample spreadsheet:
//            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
//            ValueRange response = request.Execute();
//            IList<IList<Object>> values = response.Values;
//            List<UniversityToMigratre> res = new List<UniversityToMigratre>();
//            if (values != null && values.Count > 0)
//            {

//                foreach (var row in values)
//                {
//                    Guid.TryParse(row[0].ToString(), out var uniid);
//                    //int.TryParse(row[1].ToString(), out var oldid);
//                    // Print columns A and E, which correspond to indices 0 and 4.
//                    UniversityToMigratre obj = new UniversityToMigratre()
//                    {
//                        UniId = uniid,
//                        //OldId = oldid,
//                        //Name = row[2].ToString(),
//                        NewId = row[3].ToString()
//                    };

//                    res.Add(obj);
//                }
//                return res;
//            }
//            else
//            {
//                Console.WriteLine("No data found.");
//                return null;
//            }

//        }


//        public static void WriteToUniSheet(int rowNumber)
//        {
//            UserCredential credential;

//            using (var stream =
//                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
//            {
//                string credPath = "token.json";


//                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.Load(stream).Secrets,
//                    _scopes,
//                    "user",
//                    CancellationToken.None,
//                    new FileDataStore(credPath, true)).Result;
//                Console.WriteLine("Credential file saved to: " + credPath);
//            }

//            // Create Google Sheets API service.
//            var service = new SheetsService(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = _applicationName,
//            });



//            String spreadsheetId2 = "1gkK8vARdUhwtzVGthpFdp0BZOhHY2KfZTpvN0oU94l8";
//            String range2 = $"Sheet1!E{rowNumber}";  // update cell F5 
//            ValueRange valueRange = new ValueRange();
//            valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

//            var oblist = new List<object>() { "Not Processed" };
//            valueRange.Values = new List<IList<object>> { oblist };

//            SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId2, range2);
//            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
//            UpdateValuesResponse result2 = update.Execute();

//        }


//        public static void WriteCourses(int rowNumber, string firstCourse, string secondCourse)
//        {
//            UserCredential credential;

//            using (var stream =
//                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
//            {
//                string credPath = "token.json";


//                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//                    GoogleClientSecrets.Load(stream).Secrets,
//                    _scopes,
//                    "user",
//                    CancellationToken.None,
//                    new FileDataStore(credPath, true)).Result;
//                Console.WriteLine("Credential file saved to: " + credPath);
//            }

//            // Create Google Sheets API service.
//            var service = new SheetsService(new BaseClientService.Initializer()
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = _applicationName,
//            });



//            String spreadsheetId2 = "1gU9m_InQWRkw4ZkFTPq_LnTemEBPGiBUho0cv80l-D4";
//            String range2 = $"Courses!A{rowNumber}:B";  // update cell F5 
//            ValueRange valueRange = new ValueRange();
//            valueRange.MajorDimension = "ROWS";//COLUMNS

//            var oblist = new List<object>() { firstCourse, secondCourse };
//            valueRange.Values = new List<IList<object>> { oblist };

//            SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId2, range2);
//            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
//            UpdateValuesResponse result2 = update.Execute();

//        }
//    }

//    public class CorseToMigratre
//    {
//        public string NewName { get; set; }
//        public string OldName { get; set; }
//        public string Action { get; set; }
//        public string NewId { get; set; }
//    }

//    public class UniversityToMigratre
//    {
//        public Guid UniId { get; set; }
//        //public int OldId { get; set; }
//        //public string Name { get; set; }
//        public string NewId { get; set; }
//    }
//}
