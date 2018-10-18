using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
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

        //[FunctionName("Test")]
        //public static async Task Run2Async(
        //    [TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer,
        //    [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<Mail> emailProvider)
        //{
        //    //string template = @"Hello World @Model.Name. Time is: @DateTime.Now";
        //    //var host = new RazorEngine();
        //    //string result = host.RenderTemplate(template, new { Name = "Joe Doe" });
        //    BaseEmail password = new RegistrationEmail("ram@cloudents.com", "https://www.spitball.co");

           
        //    var message = new Mail();
        //    var personalization = new Personalization();
        //    personalization.AddTo(new Email("ram@cloudents.com"));
        //    foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(password))
        //    {
        //        var p = prop.GetValue(password);
        //        personalization.AddSubstitution($"-{prop.Name}-", p?.ToString() ?? string.Empty);
        //    }
        //    //personalization.AddSubstitution("-xxx-", "{unsubscribeUrl}");
        //    message.Asm = new ASM()
        //    {
        //        GroupId = 10926
        //    };
        //    message.AddPersonalization(personalization);
        //    message.Subject = " this is a test";
        //    message.TemplateId = "e4334fe9-b71d-466f-80ea-737bf16d9c81";

        //    await emailProvider.AddAsync(message).ConfigureAwait(false);

        //}
    }
}
