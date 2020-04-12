using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using Cloudents.Core.Interfaces;

namespace ConsoleApp
{
    public class GoogleSheetsReader
    {

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] _scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string _applicationName = "Quickstart";

        public static async Task Read()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token2.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName,
            });

            // Define request parameters.
            string spreadsheetId = "19p5NTUpzDVICSCAYhqTvvjmbgU9AoInJMWwfJpfwX3A";
            string range = "Full Clourse List!A2:E972";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            var values = response.Values;

            
            //var bus = Program.Container.Resolve<ICommandBus>();

            if (values != null && values.Count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {

                    using var child = Program.Container.BeginLifetimeScope();

                    var session = child.Resolve<ISession>();
                    var unitOfWork = child.Resolve<IUnitOfWork>();
                    var row = values[i];

                    Country country = row[0].ToString();
                    var field = row[1].ToString();
                    var subject = row[2].ToString();
                    var search = row[3].ToString();
                    var teacher = row[4].ToString();

                    var course = new Course2(country, field, subject, search, teacher);
                    await session.SaveAsync(course);

                    await unitOfWork.CommitAsync(default);
                }
            }
        }
    }
}

