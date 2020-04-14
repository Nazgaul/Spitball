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
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
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
            string range = "RamISL!A2:c1660";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = await request.ExecuteAsync();
            var values = response.Values;

            
            //var bus = Program.Container.Resolve<ICommandBus>();

            if (values == null || values.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < values.Count; i++)
            {
                try
                {
                   
                    var row = values[i];
                    Console.WriteLine($"Line index {i}");
                    //Country country = row[0].ToString();

                    var newMapping = row[2].ToString().Trim('"').Replace("\\\"","\"");
                    var oldCourseName = row[0].ToString();
                    if (newMapping.Equals("N.A", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                        
                    }

                    await ProcessUsers(newMapping, oldCourseName, i);
                }
                catch (DuplicateRowException e)
                {
                }
            }
        }

        private static async Task ProcessQuestion(string newMapping, string oldCourseName, int i)
        {

            using var child = Program.Container.BeginLifetimeScope();

            var session = child.Resolve<ISession>();
            var unitOfWork = child.Resolve<IUnitOfWork>();
            //TODO change here
            var course = await session.Query<Course2>().Where(w => w.Country == Country.Israel &&
                                                                   w.SearchDisplay == newMapping)
                .SingleOrDefaultAsync();
            if (course == null)
            {
                throw new ArgumentException(newMapping);
            }


            var questionIdAlreadyInCourse = new HashSet<long>(await session.Query<Question>()
                .Where(w => w.Course2.SearchDisplay == newMapping)
                .Select(s => s.Id).ToListAsync());
            //TODO change here
            var questions = await session.Query<Question>()
               // .Fetch(f => f.User)
                .Where(w => w.Course.Id == oldCourseName && w.Status.State == ItemState.Ok &&
                            w.User.Country == Country.IsraelStr)
                .ToListAsync();
            if (questions.Count == questionIdAlreadyInCourse.Count)
            {
                return;
            }

            var needToCommit = false;
            foreach (var question in questions)
            {
                if (questionIdAlreadyInCourse.Contains(question.Id))
                {
                    continue;
                }
                needToCommit = true;
                question.Course2 = course;
                //document.AssignCourse(course);
                session.Save(question);
            }
            if (needToCommit)
            {
                Console.WriteLine($"Processing {newMapping} index {i}");
                await unitOfWork.CommitAsync(default);
            }
        }

        private static async Task ProcessDocuments(string newMapping, string oldCourseName, int i)
        {
            
            using var child = Program.Container.BeginLifetimeScope();

            var session = child.Resolve<ISession>();
            var unitOfWork = child.Resolve<IUnitOfWork>();
            //TODO change here
            var course = await session.Query<Course2>().Where(w => w.Country == Country.Israel &&
                                                                   w.SearchDisplay == newMapping)
                .SingleOrDefaultAsync();
            if (course == null)
            {
                throw new ArgumentException(newMapping);
            }


            var documentIdAlreadyInCourse =  new HashSet<long>(await session.Query<DocumentCourse>()
                .Where(w => w.Course.SearchDisplay == newMapping)
                .Select(s => s.Document.Id).ToListAsync());
            //TODO change here
            var documents = await session.Query<Document>()
                //.Fetch(f=>f.User)
                .Where(w => w.Course.Id == oldCourseName && w.Status.State == ItemState.Ok &&
                            w.User.Country == Country.IsraelStr)
                .ToListAsync();
            if (documents.Count == documentIdAlreadyInCourse.Count)
            {
                return;
            }

            var needToCommit = false;
            foreach (var document in documents)
            {
                if (documentIdAlreadyInCourse.Contains(document.Id))
                {
                    continue;
                }
                needToCommit = true;
                document.AssignCourse(course);
                session.Save(document);
            }
            if (needToCommit)
            {
                Console.WriteLine($"Processing {newMapping} index {i}");
                await unitOfWork.CommitAsync(default);
            }
        }

        private static async Task ProcessUsers(string newMapping, string oldCourseName, int i)
        {
            using var child = Program.Container.BeginLifetimeScope();

            var session = child.Resolve<ISession>();
            var unitOfWork = child.Resolve<IUnitOfWork>();

            //TODO change here
            var course = await session.Query<Course2>().Where(w => w.Country == Country.Israel 
                                                                   && w.SearchDisplay == newMapping)
                .SingleOrDefaultAsync();
            if (course == null)
            {
                throw new ArgumentException(newMapping);
            }

            var f1 =  session.Query<UserCourse2>()
                .Where(w => w.Course.SearchDisplay == newMapping)
                .Select(s => s.User.Id).ToFuture();
            var f2 = session.Query<UserCourse>()
                //.Fetch(f=>f.User)
                .Where(w => w.Course.Id == oldCourseName)
                .Where(w => w.User.Country == "IL")
                .Where(w => w.User.LockoutEnd != DateTimeOffset.MaxValue)
                .Where(w => w.User.EmailConfirmed && w.User.PhoneNumberConfirmed)
                .Select(s => new {s.User.Id, s.IsTeach}).ToFuture();

            var userIdAlreadyInCourse = new HashSet<long>(f1.GetEnumerable());

            //TODO change here
            var users = f2.GetEnumerable().ToList();
            if (userIdAlreadyInCourse.Count == users.Count)
            {
                return;
            }
            var needToCommit = false;
            foreach (var user2 in users)
            {
                if (userIdAlreadyInCourse.Contains(user2.Id))
                {
                    continue;
                }

                var user = session.Get<User>(user2.Id);

                //TODO change here
                if (user.Country != "IL")
                {
                    continue;
                }

                if (user.LockoutEnd == DateTimeOffset.MaxValue)
                {
                    continue;
                }

                if (!user.EmailConfirmed && !user.PhoneNumberConfirmed)
                {
                    continue;
                }
                needToCommit = true;
                user.AssignCourse2(course, user2.IsTeach);
                session.Save(user);
            }

            if (needToCommit)
            {
                Console.WriteLine($"Processing {newMapping} index {i}");
                await unitOfWork.CommitAsync(default);
            }
        }
    }
}

