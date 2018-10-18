using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Functions.Sync;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;
using Westwind.RazorHosting;

namespace Cloudents.Functions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Azure function")]
    public static class CourseFunction
    {
        [FunctionName("CourseSearchSync")]
        public static async Task RunAsync([TimerTrigger("0 */30 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient starter,
            TraceWriter log,
            CancellationToken token)
        {
            //const string instanceId = "CourseSearchSync";
            await SyncFunc.StartSearchSync(starter, log, SyncType.Course);
            
        }

        [FunctionName("Test")]
        public static async Task Run2Async(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<Mail> emailProvider)
        {
            //string template = @"Hello World @Model.Name. Time is: @DateTime.Now";
            //var host = new RazorEngine();
            //string result = host.RenderTemplate(template, new { Name = "Joe Doe" });

            var message = new Mail();
            var personalization = new Personalization();
            personalization.AddTo(new Email("ram@cloudents.com"));
            personalization.AddSubstitution("-FirstName-","Beny");
            message.AddPersonalization(personalization);

            message.Subject = " this is a test";
            message.TemplateId = "4f915763-1f9e-4b81-9ed7-09c1201c677b";

            await emailProvider.AddAsync(message).ConfigureAwait(false);

        }
    }
}
