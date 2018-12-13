using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Dapper;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Net.Http.Headers;
using JetBrains.Annotations;
using System.Globalization;
using RestSharp;
using RestSharp.Authenticators;

namespace Cloudents.MailgunFunctions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
    public static class MailGunFunction
    {
        private const string MailGunMailTemplateHtml = "spamgun/mail_template.html";


        [FunctionName("MailGunTest")]
        public static async Task MailGunTestAsync([TimerTrigger("0 * * * * *")]TimerInfo myTimer,
            [Blob(MailGunMailTemplateHtml,Connection = "StorageConnectionString")]
            string blob,
            CancellationToken token,
            TraceWriter log)
        {
            if (string.IsNullOrEmpty(blob))
            {
                log.Error("mailgun - Email template cannot be null");
            }
            var testUniversities = new[] { 9999 };
            await MailGunProcessAsync(testUniversities, blob, token).ConfigureAwait(false);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        [FunctionName("MailGunProcess")]
        public static async Task MailGunProcessAsync([TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [Blob(MailGunMailTemplateHtml,Connection = "StorageConnectionString")]
            string blob,
            CancellationToken token,
            TraceWriter log)
        {
            if (string.IsNullOrEmpty(blob))
            {
                log.Error("mailgun - Email template cannot be null");
            }
            
            var university = GetMailGunUniversity();
            await MailGunProcessAsync(university.Select(s => s.Id), blob, token).ConfigureAwait(false);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        private static async Task MailGunProcessAsync(
            IEnumerable<int> universities,
            string htmlBody,
            CancellationToken token)
        {
           
            const int limitPerIp = 1000;

            var limitPerSession = new[] { 5, 5, 50, 50 };

            var universityAsList = universities.ToList();
            for (var j = 0; j < limitPerSession.Length; j++)
            {
                var counter = 0;
                foreach (var universityId in universityAsList)
                {
                    if (counter >= limitPerIp)
                    {
                        break;
                    }
                    if (j == 3 && universityId == 12) //michigan detect ip number 3
                    {
                        continue;
                    }

                    var emails = MailGunQuery(universityId, limitPerSession[j]);
                   

                    var emailsTask = new List<Task>();
                    var k = 0;
                    foreach (var email in emails) //{
                    {
                        if (counter >= limitPerIp)
                        {
                            break;
                        }

                        var emailBody = GenerateMail(htmlBody, email.MailBody, email.FirstName.UppercaseFirst(), email.Email);
                        var t = new MailProvider();
                        var t1 = t.SendSpanGunEmailAsync(
                             BuildIpPool(j),
                             new MailGunRequest(email.Email, email.MailSubject, emailBody, email.MailCategory, k),
                            token);

                       

                        counter++;
                        k++;
                        emailsTask.Add(t1);
                        UpdateUser(email.Id);
                    }
                    await Task.WhenAll(emailsTask).ConfigureAwait(false);
                }
            }
        }
        private static void UpdateUser(long Id)
        {
            using (var connection = new SqlConnection(GetEnvironmentVariable("MailGunConnectionString")))
            {
                connection.Execute($@"UPDATE students2 
                                SET [Sent] = cast ( Coalesce([Sent],'') as nvarchar(max))  + cast (getUtcDate() as nvarchar(max) ) + ' ',
                                shouldSend = 0
                                where id = {Id}");
            }
            
        }

        private static string GenerateMail(string html, string body, string name, string email)
        {
            var sb = new StringBuilder(html);
            sb.Replace("{name}", name);
            sb.Replace("{email}", email);
            sb.Replace("{body}", body.Replace("\n", "<br>"));
            return sb.ToString();
        }

        private static string UppercaseFirst(this string str)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpperInvariant(str[0]) + str.Substring(1).ToLowerInvariant();
        }

        private static string BuildIpPool(int i)
        {
            if (i == 0)
            {
                return string.Empty;
            }
            return i.ToString();
        }

        private static IEnumerable<MailGunDto> MailGunQuery(long Id, int LimitPerSession)
        {
            using (var connection = new SqlConnection(GetEnvironmentVariable("MailGunConnectionString")))
            {
                var t = connection.Query<MailGunDto>($@"SELECT top {LimitPerSession} s.id, FirstName, LastName, Email,mailBody as MailBody,
                                                        mailSubject as MailSubject, mailCategory as MailCategory
                                                        from students2 s 
                                                        where uniId = {Id}
                                                        and shouldSend = 1
                                                        and chapter is null
                                                        order by s.id;");
                return t;
               
            }
        }


        public static IEnumerable<MailGunUniversityDto> GetMailGunUniversity()
        {
            using (var connection = new SqlConnection(GetEnvironmentVariable("MailGunConnectionString")))
            {
                return connection.Query<MailGunUniversityDto>(
                @"SELECT DISTINCT UniId as Id FROM dbo.students2
                WHERE ShouldSend = 1");
            }
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

    }

    internal class Student
    {
        public long Id { get; set; }
        public string Sent { get; set; }
        public bool ShouldSend { get; set; }
    }

    public class MailGunUniversityDto
    {
        public int Id { get; set; }
    }

    public class MailGunDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string MailBody { get; set; }

        public string MailSubject { get; set; }

        public string MailCategory { get; set; }
    }

    [DataContract]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "We are going via reflection")]
    public class MailGunRequest
    {
        public MailGunRequest(string to, string subject,
            string html, string tag, int interVal)
        {
            To = to;
            Subject = subject;
            Html = html;
            Tag = tag;
            _deliveryTime = DateTime.UtcNow.AddMinutes(interVal);
        }

        private DateTime _deliveryTime;

        [DataMember(Name = "from")]
        public string From => "Olivia Williams <olivia@spitball.co>";

        [DataMember(Name = "to")]
        public string To { [UsedImplicitly] get; }

        [DataMember(Name = "subject")]
        public string Subject { [UsedImplicitly] get; }

        [DataMember(Name = "html")]
        public string Html { [UsedImplicitly]get; }

        [DataMember(Name = "o:tag")]
        public string Tag { [UsedImplicitly] get; }

        [DataMember(Name = "o:campaign")]
        [UsedImplicitly]
        public string Campaign => "spamgun";

        [DataMember(Name = "o:deliverytime")]
        [UsedImplicitly]
        public string DeliverIn => _deliveryTime.ToString("ddd, dd MMM yyyy HH:mm:ss UTC");
    }


    [UsedImplicitly]
    public class MailProvider 
    {
    
        private const string MailGunApiKey = "key-5aea4c42085523a28a112c96d7b016d4";

        public async Task<IRestResponse> SendSpanGunEmailAsync(
            string ipPool,
            MailGunRequest parameters,
            CancellationToken cancellationToken)
        {
          
            var byteArray = Encoding.ASCII.GetBytes($"api:{MailGunApiKey}");
            //var authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            
            var uri = new Uri($"https://api.mailgun.net/v3/mg{ipPool}.spitball.co/messages");

               
            RestClient client = new RestClient();
            client.BaseUrl = uri;
            client.Authenticator =
                new HttpBasicAuthenticator("api", MailGunApiKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", ipPool, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"mailgun@{ipPool}");
            request.AddParameter("to", parameters.To);
            request.AddParameter("subject", parameters.Subject);
            request.AddParameter("html", parameters.Html);
            request.AddParameter("campaign", parameters.Campaign);
            request.AddParameter("deliverIn", parameters.DeliverIn);
            request.AddParameter("tag", parameters.Tag);
            request.Method = Method.POST;
            return client.Execute(request);

        }
    }
   
}
