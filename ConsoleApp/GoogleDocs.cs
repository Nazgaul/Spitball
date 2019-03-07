using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Util.Store;

namespace ConsoleApp
{
    public class GoogleDocs
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/docs.googleapis.com-dotnet-quickstart.json
        static string[] Scopes =
        {
            DocsService.Scope.Documents,
            DriveService.Scope.Drive
        };
        static string ApplicationName = "Google Docs API .NET Quickstart";

        public static void Main()
        {
            var docCred = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("tutoring-docs@bsnlp-158707.iam.gserviceaccount.com")
            {
                Scopes = Scopes
            }.FromPrivateKey("-----BEGIN PRIVATE KEY-----\nMIIEuwIBADANBgkqhkiG9w0BAQEFAASCBKUwggShAgEAAoIBAQDCcORyRfjah76l\nghRGKkHsTh9hUjqP31h7fWvvk0PM1lArK5iKXeGyQHMM4Rhkr8ojkwj/QsYq9PuB\nVjQnE/l2uXmgvECWZGW4rea3JbfFdlFIyjy0T4Y0pvkORQ5qukrTJNELT6/qqvzZ\nBF2VPrzGLKE3DSjO2rN5F5Vr3ctyVCRRRper7JpLY3m5OVNgxaCjUtk3ebnQshPD\nRGmH/n/LiLM9dowxNwfFByCmgBR8/Ym1qTkNb1ohkOwpl20Aaoluwx9hq43STFeR\n+ovLd4n3a32VboVt9/d85JKP72YpaO3l2QT9H3vP+eNKWWk8d0BGl6SDPb/cVUGs\nzIHW9sc7AgMBAAECgf8Km74GRFtEmWMQtDvPgCguO7mSmoiLZCvrd6QzbMcFS4bJ\nrutbhZYQdCDKYdGvlJTEcvCvYYkXxBN1VSVyc1jRNdsq1WnV8l/PQ9tO+KtoOwZR\nITlYtR1EamWJ8f560v+r2ryHALUnXjGl2ldsAzBTD2DL9XJzZbwhrxp95PAS+b6j\nKW/LHqUWvTWbn+WwdgHC5cBcUW8p4K24nJPaG94KFCrMmLq5m0a63NqxnygFArMq\necrvFix+pglMgFVI3Sc0fcXpwvtS2ac37a7jjoX7YWQLagDvQUeMIqTmiR3Rq6K6\n4J2z63eLu0fmZeyiRrXX9+3EDG0p61pDnn4td7kCgYEA/PsQ9RxL7szPdbHA+ggn\nPivsNZnczIUzvKt09FnHQGs+iTDZGBoBf+xS+CB4MdttewgSoBEU7rt1mu13Tk3H\n26fjWv/31FeXkWmXJh6L+g2H8cHrJlStoqGLGScCGgHSZpPXq+PTVHKfeQfHRmHG\nhugf9FVbHHnFgLFiSOW5JZ0CgYEAxML4IO2M7uwNeVLLgjtsG0XeOycjZczKSiOH\nYVMHIG9NsVHTnUPoUb/Ibb9OspBkUrGC9j3l6WRJABtLx+yi6nhJflL+92vjUl2p\nskCTCFINVSOAmCCYwg/7WjRpVBxvN3CzaXvmbZELad703yXWIO2w8/PJbhnhErkS\nhBcNNLcCgYEAtpxYqkYJvc/TtUZhYVq/UQ6NjEeLbYp9RSSS4Mtpm+OOGaPMIays\nBnZFkdyGRp75EUVLhIWwEX/5raLaawiUSseOwyJf98ReoVWSCK8mJZLc3bM4Fcws\nJmfrq3VP/Avyn576oJZs9tliqg/mVbTAhKNMZaezhAOWgIYKx0oj8rUCgYBHM9be\n85fgjgOCN/f1NNO2Ot5kwZEJscYydhDzozwc/Ko81MOjUYOssZ4yONyduarUUfB2\nc4fTobrZEwelXXjHKKhP0nD50Ez+7W4PV1bd1/ODL8nFQ2aEM4xd5EGJcpC8m0gR\nkAie9bIPqeMrLWIDoIA8h6gI88yDHf4ZVs4smQKBgDoM7hrm32YFeLEupMlv8tSs\nK1vaeVRDNqUHybuMUjEsr/RuDskMf/GuqDB29gUPza+V2NolC185pOKG9dCXzcCj\nK8hqa7chHNI27eOgcFZElbgFUfT3jFMG2uSGcBYE+SD9cydIYZg9Vsk9FUrzWvpy\ngvcm8PJdfPGEpklJtpEu\n-----END PRIVATE KEY-----\n"));
            //UserCredential credential;
            //using (var stream =
            //    new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            //{
            //    string credPath = "token.json";
               

            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
            //    Console.WriteLine("Credential file saved to: " + credPath);
            //}

            // Create Google Docs API service.
            var service = new DocsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = docCred,
                ApplicationName = ApplicationName,
            });
            var driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = docCred,
                ApplicationName = ApplicationName,
            });

            var doc = new Document()
            {
                Title = "ram test2",
               

            };
            
            var request = service.Documents.Create(doc);

            doc = request.Execute();

            driveService.Permissions.Create(new Permission()
            {
                Role = "writer",
                Type = "anyone"
            }, doc.DocumentId).Execute();

            var request2 = driveService.Files.Get(doc.DocumentId);
            request2.Fields = "id,webViewLink";
            var webLink = request2.Execute().WebViewLink;
            



            //DocumentsResource.GetRequest request = service.Documents.Get(documentId);

            // Prints the title of the requested doc:
            // https://docs.google.com/document/d/195j9eDD3ccgjQRttHhJPymLJUCOUjs-jmwTrekvdjFE/edit
            //Document doc = request.Execute();
            Console.WriteLine("The title of the doc is: {0}", doc.Title);
        }
    }
}