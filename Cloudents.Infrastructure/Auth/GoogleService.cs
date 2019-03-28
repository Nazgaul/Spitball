using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace Cloudents.Infrastructure.Auth
{
    [UsedImplicitly]
    public sealed class GoogleService : IGoogleAuth, IGoogleDocument
    {
        public async Task<ExternalAuthDto> LogInAsync(string jwt, CancellationToken cancellationToken)
        {

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] {"341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com"}
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

        private static readonly string[] Scopes =
        {
            DocsService.Scope.Documents,
            DriveService.Scope.Drive
        };

        public async Task<string> CreateOnlineDocAsync(string documentName,CancellationToken token)
        {
            var docCred = new ServiceAccountCredential(new ServiceAccountCredential.Initializer("tutoring-docs@bsnlp-158707.iam.gserviceaccount.com")
            {
                Scopes = Scopes
            }.FromPrivateKey("-----BEGIN PRIVATE KEY-----\nMIIEuwIBADANBgkqhkiG9w0BAQEFAASCBKUwggShAgEAAoIBAQDCcORyRfjah76l\nghRGKkHsTh9hUjqP31h7fWvvk0PM1lArK5iKXeGyQHMM4Rhkr8ojkwj/QsYq9PuB\nVjQnE/l2uXmgvECWZGW4rea3JbfFdlFIyjy0T4Y0pvkORQ5qukrTJNELT6/qqvzZ\nBF2VPrzGLKE3DSjO2rN5F5Vr3ctyVCRRRper7JpLY3m5OVNgxaCjUtk3ebnQshPD\nRGmH/n/LiLM9dowxNwfFByCmgBR8/Ym1qTkNb1ohkOwpl20Aaoluwx9hq43STFeR\n+ovLd4n3a32VboVt9/d85JKP72YpaO3l2QT9H3vP+eNKWWk8d0BGl6SDPb/cVUGs\nzIHW9sc7AgMBAAECgf8Km74GRFtEmWMQtDvPgCguO7mSmoiLZCvrd6QzbMcFS4bJ\nrutbhZYQdCDKYdGvlJTEcvCvYYkXxBN1VSVyc1jRNdsq1WnV8l/PQ9tO+KtoOwZR\nITlYtR1EamWJ8f560v+r2ryHALUnXjGl2ldsAzBTD2DL9XJzZbwhrxp95PAS+b6j\nKW/LHqUWvTWbn+WwdgHC5cBcUW8p4K24nJPaG94KFCrMmLq5m0a63NqxnygFArMq\necrvFix+pglMgFVI3Sc0fcXpwvtS2ac37a7jjoX7YWQLagDvQUeMIqTmiR3Rq6K6\n4J2z63eLu0fmZeyiRrXX9+3EDG0p61pDnn4td7kCgYEA/PsQ9RxL7szPdbHA+ggn\nPivsNZnczIUzvKt09FnHQGs+iTDZGBoBf+xS+CB4MdttewgSoBEU7rt1mu13Tk3H\n26fjWv/31FeXkWmXJh6L+g2H8cHrJlStoqGLGScCGgHSZpPXq+PTVHKfeQfHRmHG\nhugf9FVbHHnFgLFiSOW5JZ0CgYEAxML4IO2M7uwNeVLLgjtsG0XeOycjZczKSiOH\nYVMHIG9NsVHTnUPoUb/Ibb9OspBkUrGC9j3l6WRJABtLx+yi6nhJflL+92vjUl2p\nskCTCFINVSOAmCCYwg/7WjRpVBxvN3CzaXvmbZELad703yXWIO2w8/PJbhnhErkS\nhBcNNLcCgYEAtpxYqkYJvc/TtUZhYVq/UQ6NjEeLbYp9RSSS4Mtpm+OOGaPMIays\nBnZFkdyGRp75EUVLhIWwEX/5raLaawiUSseOwyJf98ReoVWSCK8mJZLc3bM4Fcws\nJmfrq3VP/Avyn576oJZs9tliqg/mVbTAhKNMZaezhAOWgIYKx0oj8rUCgYBHM9be\n85fgjgOCN/f1NNO2Ot5kwZEJscYydhDzozwc/Ko81MOjUYOssZ4yONyduarUUfB2\nc4fTobrZEwelXXjHKKhP0nD50Ez+7W4PV1bd1/ODL8nFQ2aEM4xd5EGJcpC8m0gR\nkAie9bIPqeMrLWIDoIA8h6gI88yDHf4ZVs4smQKBgDoM7hrm32YFeLEupMlv8tSs\nK1vaeVRDNqUHybuMUjEsr/RuDskMf/GuqDB29gUPza+V2NolC185pOKG9dCXzcCj\nK8hqa7chHNI27eOgcFZElbgFUfT3jFMG2uSGcBYE+SD9cydIYZg9Vsk9FUrzWvpy\ngvcm8PJdfPGEpklJtpEu\n-----END PRIVATE KEY-----\n"));

            var service = new DocsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = docCred,
            });
            var driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = docCred,
            });

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
}
