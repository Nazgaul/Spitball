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
using Microsoft.WindowsAzure.Storage.Blob;
using static System.Threading.Tasks.Task;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class MailGunFunction
    {
        [FunctionName("MailGunTest")]
        [UsedImplicitly]
        public static async Task MailGunTestAsync([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Inject] IReadRepositoryAsync<IEnumerable<MailGunDto>, long> dataRepository,
            [Inject] IMailProvider mailProvider,
            [Inject] ICommandBus bus,
            [Blob("spamgun/mail-template.html")]
            string blob,
            CancellationToken token,
            TraceWriter log)
        {
            var testUniversities = new[] { 38 };
            await MailGunProcessAsync(testUniversities, dataRepository, mailProvider, bus, blob, token);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        //[FunctionName("MailGunProcess")]
        public static async Task MailGunProcessAsync([TimerTrigger("0 0 */1 * * *")]TimerInfo myTimer,
            [Inject]IReadRepositoryAsync<IEnumerable<MailGunUniversityDto>> universityRepository,
            [Inject] IReadRepositoryAsync<IEnumerable<MailGunDto>, long> dataRepository,
            [Inject] IMailProvider mailProvider,
            [Inject] ICommandBus bus,
            [Blob("spamgun/mail-template.html")]
            string blob,
            CancellationToken token,
            TraceWriter log)
        {
            var university = await universityRepository.GetAsync(token);
            await MailGunProcessAsync(university.Select(s => s.Id), dataRepository, mailProvider, bus, blob, token);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }


        private static async Task MailGunProcessAsync(
            //IReadRepositoryAsync<IEnumerable<MailGunUniversityDto>> universityRepository,
            IEnumerable<int> universities,
            IReadRepositoryAsync<IEnumerable<MailGunDto>, long> dataRepository,
            IMailProvider mailProvider,
            ICommandBus bus,
            string htmlBody,
            CancellationToken token)
        {
            const int numberOfIps = 4;
            const int limitPerIp = 1000;
            //var numberOfUniversitiesTask = universityRepository.GetAsync(token);
            // var htmlBody = await  blob.DownloadTextAsync(token).ConfigureAwait(false);
            //await WhenAll(numberOfUniversitiesTask, htmlBodyTask).ConfigureAwait(false);
            //var numberOfUniversities = numberOfUniversitiesTask.Result.ToList();
            //var htmlBody = htmlBodyTask.Result;
            var universityAsList = universities.ToList();
            for (var j = 0; j < numberOfIps; j++)
            {
                var counter = 0;
                foreach (var universityId in universityAsList)
                {
                    //for (var i = 0; i < numberOfUniversities; i++)
                    //{
                    if (counter >= limitPerIp)
                    {
                        break;
                    }
                    if (j == 3 && universityId == 12) //umich detect ip number 3
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

                        var emailBody = GenerateMail(htmlBody, email.MailBody, email.FirstName.UppercaseFirst());
                        var t1 = mailProvider.SendSpanGunEmailAsync(
                             BuildIpPool(j),
                             new MailGunRequest(email.Email, email.MailSubject, emailBody, email.MailCategory, k),
                            //new MailGunRequest(email.MailBody,
                            //    email.FirstName.UppercaseFirst(), email.MailSubject,
                            //    email.MailCategory, htmlBody),
                            //k,
                            token);
                        var command = new UpdateMailGunCommand(email.Id);
                        await bus.DispatchAsync(command, token).ConfigureAwait(false);
                        //await _zboxWriteService.UpdateSpamGunSendAsync(email.Id, token).ConfigureAwait(false);
                        counter++;
                        k++;
                        emailsTask.Add(t1);
                    }
                    await WhenAll(emailsTask).ConfigureAwait(false);
                }
            }
        }

        private static string GenerateMail(string html, string body, string name)
        {
            var sb = new StringBuilder(html);
            sb.Replace("{name}", name);
            sb.Replace("{body}", body.Replace("\n", "<br>"));
            //html.Replace("{uni_Url}", _parameters.UniversityUrl);
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
