﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Docs.v1;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Exceptions;
using Cloudents.Infrastructure.Google.Resources;
using Google.Apis;
using Document = Google.Apis.Docs.v1.Data.Document;
using Google.Apis.Auth.OAuth2.Responses;
using Channel = Google.Apis.Calendar.v3.Data.Channel;

namespace Cloudents.Infrastructure.Google
{
    [UsedImplicitly]
    public sealed class GoogleService :
        IGoogleAuth,
        IGoogleDocument,
        ICalendarService
    {
        private const string PrimaryGoogleCalendarId = "primary";
        private readonly ILifetimeScope _container;

        public GoogleService(ILifetimeScope container)
        {
            _container = container;
        }


        //public GoogleService(GoogleDataStore googleDataStore)
        //{
        //    _googleDataStore = googleDataStore;
        //}

        public async Task<ExternalAuthDto> LogInAsync(string jwt, CancellationToken cancellationToken)
        {

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { "341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com" }
            };
            var result = await GoogleJsonWebSignature.ValidateAsync(jwt, settings);


            if (result == null)
            {
                return null;
            }

            if (!result.EmailVerified)
            {
                return null;
            }

            return new ExternalAuthDto()
            {
                Id = result.Subject,
                FirstName = result.GivenName,
                LastName = result.FamilyName,
                Email = result.Email,
                Language = result.Locale,
                Name = result.Name,
                Picture = result.Picture
            };
        }



        public async Task<string> CreateOnlineDocAsync(string documentName, CancellationToken token)
        {
            var docCred = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("tutoring-docs@bsnlp-158707.iam.gserviceaccount.com")
            {
                Scopes = new[] {
                    DocsService.Scope.Documents,
                    DriveService.Scope.Drive
                }
            }.FromPrivateKey("-----BEGIN PRIVATE KEY-----\nMIIEuwIBADANBgkqhkiG9w0BAQEFAASCBKUwggShAgEAAoIBAQDCcORyRfjah76l\nghRGKkHsTh9hUjqP31h7fWvvk0PM1lArK5iKXeGyQHMM4Rhkr8ojkwj/QsYq9PuB\nVjQnE/l2uXmgvECWZGW4rea3JbfFdlFIyjy0T4Y0pvkORQ5qukrTJNELT6/qqvzZ\nBF2VPrzGLKE3DSjO2rN5F5Vr3ctyVCRRRper7JpLY3m5OVNgxaCjUtk3ebnQshPD\nRGmH/n/LiLM9dowxNwfFByCmgBR8/Ym1qTkNb1ohkOwpl20Aaoluwx9hq43STFeR\n+ovLd4n3a32VboVt9/d85JKP72YpaO3l2QT9H3vP+eNKWWk8d0BGl6SDPb/cVUGs\nzIHW9sc7AgMBAAECgf8Km74GRFtEmWMQtDvPgCguO7mSmoiLZCvrd6QzbMcFS4bJ\nrutbhZYQdCDKYdGvlJTEcvCvYYkXxBN1VSVyc1jRNdsq1WnV8l/PQ9tO+KtoOwZR\nITlYtR1EamWJ8f560v+r2ryHALUnXjGl2ldsAzBTD2DL9XJzZbwhrxp95PAS+b6j\nKW/LHqUWvTWbn+WwdgHC5cBcUW8p4K24nJPaG94KFCrMmLq5m0a63NqxnygFArMq\necrvFix+pglMgFVI3Sc0fcXpwvtS2ac37a7jjoX7YWQLagDvQUeMIqTmiR3Rq6K6\n4J2z63eLu0fmZeyiRrXX9+3EDG0p61pDnn4td7kCgYEA/PsQ9RxL7szPdbHA+ggn\nPivsNZnczIUzvKt09FnHQGs+iTDZGBoBf+xS+CB4MdttewgSoBEU7rt1mu13Tk3H\n26fjWv/31FeXkWmXJh6L+g2H8cHrJlStoqGLGScCGgHSZpPXq+PTVHKfeQfHRmHG\nhugf9FVbHHnFgLFiSOW5JZ0CgYEAxML4IO2M7uwNeVLLgjtsG0XeOycjZczKSiOH\nYVMHIG9NsVHTnUPoUb/Ibb9OspBkUrGC9j3l6WRJABtLx+yi6nhJflL+92vjUl2p\nskCTCFINVSOAmCCYwg/7WjRpVBxvN3CzaXvmbZELad703yXWIO2w8/PJbhnhErkS\nhBcNNLcCgYEAtpxYqkYJvc/TtUZhYVq/UQ6NjEeLbYp9RSSS4Mtpm+OOGaPMIays\nBnZFkdyGRp75EUVLhIWwEX/5raLaawiUSseOwyJf98ReoVWSCK8mJZLc3bM4Fcws\nJmfrq3VP/Avyn576oJZs9tliqg/mVbTAhKNMZaezhAOWgIYKx0oj8rUCgYBHM9be\n85fgjgOCN/f1NNO2Ot5kwZEJscYydhDzozwc/Ko81MOjUYOssZ4yONyduarUUfB2\nc4fTobrZEwelXXjHKKhP0nD50Ez+7W4PV1bd1/ODL8nFQ2aEM4xd5EGJcpC8m0gR\nkAie9bIPqeMrLWIDoIA8h6gI88yDHf4ZVs4smQKBgDoM7hrm32YFeLEupMlv8tSs\nK1vaeVRDNqUHybuMUjEsr/RuDskMf/GuqDB29gUPza+V2NolC185pOKG9dCXzcCj\nK8hqa7chHNI27eOgcFZElbgFUfT3jFMG2uSGcBYE+SD9cydIYZg9Vsk9FUrzWvpy\ngvcm8PJdfPGEpklJtpEu\n-----END PRIVATE KEY-----\n"));

            var clientService = new BaseClientService.Initializer()
            {
                HttpClientInitializer = docCred,

            };
            using (var service = new DocsService(clientService))
            using (var driveService = new DriveService(clientService))
            {

                var doc = new Document()
                {
                    Title = documentName
                };
                var request = service.Documents.Create(doc);

                doc = await request.ExecuteAsync(token);

                await driveService.Permissions.Create(new Permission()
                {
                    Role = "writer",
                    Type = "anyone"
                }, doc.DocumentId).ExecuteAsync(token);

                var request2 = driveService.Files.Get(doc.DocumentId);
                request2.Fields = "id,webViewLink";
                var webLink = (await request2.ExecuteAsync(token)).WebViewLink;

                return webLink;
            }
        }

        private static GoogleClientSecrets GetGoogleClientSecrets()
        {
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Cloudents.Infrastructure.Google.calendar.json"))
            {
                return GoogleClientSecrets.Load(stream);
            }
        }




        public async Task<(IEnumerable<CalendarEventDto>, string etag)> ReadCalendarEventsAsync(long userId,
            DateTime from, DateTime max,
            CancellationToken cancellationToken)
        {
            //var googleDataStore = _container.Resolve<GoogleDataStore>();
            using (var child = _container.BeginLifetimeScope())
            {
                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = GetGoogleClientSecrets().Secrets,
                    Scopes = new[] { CalendarService.Scope.CalendarReadonly },
                    DataStore = child.Resolve<GoogleDataStore>()
                };
                //TODO: need to find solution We can't dispose the flow because we are using it in the code 
                var flow = new GoogleAuthorizationCodeFlow(initializer);

                var gToken = await flow.LoadTokenAsync(userId.ToString(), cancellationToken);
                if (gToken == null) throw new NotFoundException(nameof(gToken));
                var credential = new UserCredential(flow, userId.ToString(), gToken);




                // var credential = await LoadUserTokenAsync(userId, cancellationToken);
                using (var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential

                }))
                {
                    var request = service.Events.List(PrimaryGoogleCalendarId);
                    if (from < DateTime.UtcNow)
                    {
                        from = DateTime.UtcNow;
                    }

                    request.SingleEvents = true;
                    request.TimeMin = from;
                    request.TimeMax = max;
                    try
                    {
                        var result = await request.ExecuteAsync(cancellationToken);
                        return (result.Items.Select(s =>
                       {
                           if (s.Start.DateTime.HasValue)
                           {
                               var startAppointmentTime = s.Start.DateTime.Value;
                               startAppointmentTime = startAppointmentTime.AddMinutes(-s.Start.DateTime.Value.Minute);
                               var endAppointmentTime = s.End.DateTime.GetValueOrDefault();
                               if (endAppointmentTime.Minute > 0)
                               {
                                   endAppointmentTime = endAppointmentTime.AddHours(1)
                                       .AddMinutes(-endAppointmentTime.Minute);
                               }

                               return new CalendarEventDto(startAppointmentTime,
                                   endAppointmentTime);
                           }

                           var start = DateTime.ParseExact(s.Start.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                           var end = DateTime.ParseExact(s.End.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                           return new CalendarEventDto(start, end);

                       }), result.ETag);
                    }
                    catch (TokenResponseException e)
                    {
                        throw new NotFoundException("Google token invalid", e);
                    }
                }
            }
        }



        public async Task BookCalendarEventAsync(
            IEnumerable<Core.Entities.User> users, DateTime from, DateTime to,
            CancellationToken cancellationToken)
        {
            var cred = SpitballCalendarCred;
            var x = new System.Resources.ResourceManager(typeof(CalendarResources));
            var eventName = x.GetString("TutorCalendarMessage", CultureInfo.CurrentUICulture);

            using (var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred
            }))
            {
                var attendees = users.Select(s => new EventAttendee()
                {
                    Email = s.Email

                }).ToList();


                var event2 = service.Events.Insert(new Event()
                {

                    Attendees = attendees,
                    Summary = eventName,
                    Start = new EventDateTime()
                    {
                        DateTime = from
                    },
                    End = new EventDateTime()
                    {
                        DateTime = to
                    }
                }, PrimaryGoogleCalendarId);
                event2.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All;
                await event2.ExecuteAsync(cancellationToken);
            }

        }

        private static ServiceAccountCredential SpitballCalendarCred
        {
            get
            {
                var cred = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("114966733453169842650")
                {
                    Scopes = new[] { CalendarService.Scope.Calendar },
                    User = "schedule@spitball.co"
                }.FromPrivateKey(
                    "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC6j5Wb+58PaIlE\nlWeitrRDKWo7gDgw3BgKCK1MA9IZd7nI5gAQDoE6X/UTFt2PHD4QtQU5juNVSS+1\nhgppSAZIHlCKiUKnNg4X5FmODk3XlrZSWsTINLosVl3W9f1tntnL2ll518NTfsv5\nCSf0D/X6+wwJ/QbgTeX2miyCfZlOsU7ARlYeXMQgPiizebVQZ4OKFETbPBf694U3\nHm7aheRPMBYb2ZKNuzJVGvi8P6IWsTgAdWb9VwrBDWgWTju2x08ux2mnmgL09ZAw\ni2KD3dQ9KfeLv/XIWcHPCRLJsnpcHABuBlr83XUrMKMt//VvN60NgmIbnZc19b4P\nwyZr8qgTAgMBAAECggEACBY8RSWBmMCOdAoeksI6YHpARXGtPd+fLSQreug5fcAf\nrgfbndaQdHrUsUNO+aqe6B4oqqeOeX5xfSa2dyeCRN2yM7x3xq7EVUshNKlbEiAm\n1BK6L/bVJncx8c+k8NEEyN40qGBEHRpEdjhBjUX52Ct0s07Osv9oG1SbNFGEc+bk\nTHTZVXYGSJRkyo3BNLohXo+pFgIdWrzD5zaURCQwxNJPJRHA3GRSVDIaQ7ik9PXK\nqm/8+rrQAv4oAifbYxF0KWI7Sj2q9JLFCfXJXmIIMq/n4l5NcpVKJstvGWe7qYFT\ne4uCn8ItVxpTJWBRDv8oL6NUsgZog0eK82/PKYucuQKBgQDylaw4zXykUzLnGh9G\nadnAVNFnxA8Yh3PwAL5V+qRXMbIWxqeOFxMeg3UDjEaUXAmn0J3Y038xm7Bsp+FY\nmJDWi77aCNvZKOs227tmLTFOEZZ1BDvgaNU+gT7XXRQWABc+5YxR/bLHan6vDnYP\nyh21BEiruUQTbdmn1kumA/QuqQKBgQDE4MUeNEMMPJrKQyPBYw/CRk4dW1/R2gX7\nEttcSk9g9UObNizXlqVPHNcDfISreuBPFq+sE6npxkraobKSYdulLtsFxw7MSBe4\niA7mppltdYknwms+VyVJL//cGQXOqG/IcEBoIiJN4DSNUIjC8ArMVYF5OXkzm6X0\nw4suwrrCWwKBgQDmR+keXwr0XzqSIb0QtckNCDdlXrvJ2EPZ0IreybkaQMXDUz+Z\n5hOzQq1g+dfCXICZ+rLtMxCqghX/f3qvBN1xnWVGS2SQCIUJJZwHCd2lM5L1cFh6\n1mmgFUcXYHeBzwJCJdyHtOLy5Qhvm7W9lWuP/AoUYiHao8wbxJU5esVhSQKBgQCO\nf43NBdC9q6Px39SiZZwDZrWlY/yfvGl1x7lEPHjl2b/cOMMOK/hsoZgy6s5v+5kd\nRXNTXkwua5rEUiMY9oFvNtHKhcB9NXUN2FTIty733gmu4HaVAah4J6jOWsIsSRfX\ngP/tHz+rFCuVWQQT7IA0U3NKFcJXC0J8PYihCMr6XwKBgH6NQiVYrg6Z6E1+T2+K\nWwfKem7uwDMi1FSi1IG9++nN92eO7ZZka4YEZ7runa4DS9a1oA6kIipT1JxAAQzi\nwhODOWc47SrdWlj9TwZm1ky8SAHvaxvECDJenWr+ooXzmdAiApnEHxQt4VsxuYhz\neLf0iFOJEWE47mX1CjBYuiSA\n-----END PRIVATE KEY-----\n"));
                return cred;
            }
        }

        public async Task SaveTokenAsync(string token, long userId, string uri,
            CancellationToken cancellationToken)
        {
            using (var child = _container.BeginLifetimeScope())
            {

                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = GetGoogleClientSecrets().Secrets,
                    Scopes = new[] { CalendarService.Scope.CalendarReadonly },
                    DataStore = child.Resolve<GoogleDataStore>()

                };

                using (var flow = new GoogleAuthorizationCodeFlow(initializer))
                {
                    await flow.ExchangeCodeForTokenAsync(userId.ToString(), token,
                        uri, // need to be in from google console
                          cancellationToken);
                }
            }


        }

        public async Task CreateWatch()
        {
            var cred = SpitballCalendarCred;

            using (var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred
            }))
            {
                var result = await service.Events.Watch(new Channel
                {
                    Type = "web_hook",
                    Address = "https://spitball-function-dev2.azurewebsites.net/api/google/notifications"
                },PrimaryGoogleCalendarId).ExecuteAsync();
            }
        }
    }
}
