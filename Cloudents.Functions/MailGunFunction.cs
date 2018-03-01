using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class MailGunFunction
    {
        private const string MailGunMailTemplateHtml = "spamgun/mail_template.html";

        [FunctionName("MailGunTest")]
        [UsedImplicitly]
        public static async Task MailGunTestAsync([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Inject] IReadRepositoryAsync<IEnumerable<MailGunDto>, long> dataRepository,
            [Inject] IMailProvider mailProvider,
            [Inject] ICommandBus bus,
            [Blob(MailGunMailTemplateHtml)]
            string blob,
            CancellationToken token,
            TraceWriter log)
        {
            var testUniversities = new[] { 9999 };
            await MailGunProcessAsync(testUniversities, dataRepository, mailProvider, bus, blob, token).ConfigureAwait(false);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        [FunctionName("MailGunProcess")]
        [UsedImplicitly]
        public static async Task MailGunProcessAsync([TimerTrigger("0 0 */1 * * *")]TimerInfo myTimer,
            [Inject]IReadRepositoryAsync<IEnumerable<MailGunUniversityDto>> universityRepository,
            [Inject] IReadRepositoryAsync<IEnumerable<MailGunDto>, long> dataRepository,
            [Inject] IMailProvider mailProvider,
            [Inject] ICommandBus bus,
            [Blob(MailGunMailTemplateHtml)]
            string blob,
            CancellationToken token,
            TraceWriter log)
        {
            var university = await universityRepository.GetAsync(token).ConfigureAwait(false);
            await MailGunProcessAsync(university.Select(s => s.Id), dataRepository, mailProvider, bus, blob, token).ConfigureAwait(false);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        private static async Task MailGunProcessAsync(
            IEnumerable<int> universities,
            IReadRepositoryAsync<IEnumerable<MailGunDto>, long> dataRepository,
            IMailProvider mailProvider,
            ICommandBus bus,
            string htmlBody,
            CancellationToken token)
        {
            const int numberOfIps = 4;
            const int limitPerIp = 1000;
            var universityAsList = universities.ToList();
            for (var j = 0; j < numberOfIps; j++)
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

                    var emails = await dataRepository.GetAsync(universityId, token).ConfigureAwait(false);

                    var emailsTask = new List<Task>();
                    var k = 0;
                    foreach (var email in emails) //{
                    {
                        if (counter >= limitPerIp)
                        {
                            break;
                        }

                        var emailBody = GenerateMail(htmlBody, email.MailBody, email.FirstName.UppercaseFirst(), email.Email);
                        var t1 = mailProvider.SendSpanGunEmailAsync(
                             BuildIpPool(j),
                             new MailGunRequest(email.Email, email.MailSubject, emailBody, email.MailCategory, k),
                            token);
                        var command = new UpdateMailGunCommand(email.Id);
                        await bus.DispatchAsync(command, token).ConfigureAwait(false);
                        counter++;
                        k++;
                        emailsTask.Add(t1);
                    }
                    await Task.WhenAll(emailsTask).ConfigureAwait(false);
                }
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


        private static string BuildIpPool(int i)
        {
            if (i == 0)
            {
                return string.Empty;
            }
            return i.ToString();
        }
    }
}
