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
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

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
            string spreadsheetId = "1hEET4d0YKmHBWQhbwkxPifcXVB5zqgs2Tcs2NiXLnlc";
            string range = "Ram Sheet!A1:C";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            var values = response.Values;

            var session = Program._container.Resolve<IStatelessSession>();
            var bus = Program._container.Resolve<ICommandBus>();

            if (values != null && values.Count > 0)
            {

                foreach (var row in values)
                {

                    var deleteRow = row.Count > 2 ? row[2].ToString() : null;

                    if (string.Equals(deleteRow, "DELETE", StringComparison.OrdinalIgnoreCase))
                    {


                        var courseName = row[0].ToString();
                        //if (courseName.ToUpperInvariant()[0] <= 'U')
                        //{
                        //     continue;
                        //}

                        Console.WriteLine($"Processing {courseName}");
                        var result = await session.Query<Course>()
                            .Where(w => w.Id == courseName).SingleOrDefaultAsync();
                        if (result is null)
                        {
                            continue;
                        }

                        var documents = await session.Query<Document>()
                            .Where(w => w.Course.Id == courseName).ToListAsync();


                        foreach (var document in documents.Where(w => w.Status.State == ItemState.Deleted))
                        {
                            await session.CreateSQLQuery("delete from sb.DocumentsTags where DocumentId = :id")
                                .SetInt64("id", document.Id).ExecuteUpdateAsync();
                            await session.Query<Document>().Where(w => w.Id == document.Id)
                                .DeleteAsync(CancellationToken.None);
                        }

                        if (documents.Count(a => a.Status.State == ItemState.Ok) > 5)
                        {
                            Console.WriteLine("Cant delete because of document");
                            continue;
                        }
                        //var canDelete = true;
                        foreach (var document in documents)
                        {
                            //var response2 = ConsoleKey.Y;
                            //if (document.Status.State == ItemState.Ok)
                            //{

                            //    Console.WriteLine($"Delete document id : {document.Id}");
                            //    response2 = Console.ReadKey(false).Key;
                            //}

                            //if (response2 == ConsoleKey.Y)
                            //{
                            var deleteDocumentCommand = new DeleteDocumentCommand(document.Id);
                            await bus.DispatchAsync(deleteDocumentCommand, default);

                            await session.CreateSQLQuery("delete from sb.DocumentsTags where DocumentId = :id")
                                .SetInt64("id", document.Id).ExecuteUpdateAsync();
                            await session.Query<Document>().Where(w => w.Id == document.Id)
                                .DeleteAsync(CancellationToken.None);
                            //}
                            //else
                            //{
                            //    canDelete = false;
                            //}
                        }


                        var questions = await session.Query<Question>().Where(w => w.Course.Id == courseName).ToListAsync();
                        if (questions.Any())
                        {
                            Console.WriteLine("Cant delete question");
                            continue;
                        }
                        //foreach (var question in questions)
                        //{
                        //    var response2 = ConsoleKey.Y;
                        //    if (question.Status.State == ItemState.Ok)
                        //    {
                        //        Console.WriteLine($"Delete document id : {question.Id} text {question.Text}");
                        //        response2 = Console.ReadKey(false).Key;
                        //    }


                        //    if (response2 == ConsoleKey.Y)
                        //    {
                        //        var deleteQuestionCommand =
                        //            new Cloudents.Command.Command.Admin.DeleteQuestionCommand(question.Id);
                        //        await bus.DispatchAsync(deleteQuestionCommand, default);

                        //        //await session.CreateSQLQuery("delete from sb.DocumentsTags where DocumentId = :id")
                        //        //    .SetInt64("id", document.Id).ExecuteUpdateAsync();
                        //        await session.Query<Answer>().Where(w => w.Question.Id == question.Id)
                        //            .DeleteAsync(CancellationToken.None);
                        //        await session.Query<Question>().Where(w => w.Id == question.Id)
                        //            .DeleteAsync(CancellationToken.None);
                        //    }
                        //    else
                        //    {
                        //        canDelete = false;
                        //    }

                        //}

                        //if (canDelete)
                        //{
                        using (var beginLifetimeScope = Program._container.BeginLifetimeScope())
                        {


                            using (var session2 = beginLifetimeScope.Resolve<ISession>())
                            {
                                using (var uc = session2.BeginTransaction())
                                {
                                    //using (var uc = Program._container.Resolve<IUnitOfWork>())
                                    //{
                                    var course = await session2.LoadAsync<Course>(courseName);
                                    await session2.DeleteAsync(course);

                                    //await session.Query<Course>().Where(w => w.Id == courseName).DeleteAsync(CancellationToken.None);

                                    await uc.CommitAsync(CancellationToken.None);
                                }

                                //}
                            }
                        }
                        //}

                    }
                }
            }


        }
    }





}

