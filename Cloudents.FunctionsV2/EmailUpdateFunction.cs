using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.FunctionsV2.System;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class EmailUpdateFunction
    {
        [FunctionName("EmailUpdateFunction")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            [Inject] IQueryBus queryBus,
            CancellationToken token)
        {
            //Run Every 1 day

            //TODO add paging
            var query = new GetUpdatesEmailUsersQuery();
            var result = await queryBus.QueryAsync(query, token);

            foreach (var emailDto in result)
            {
                await context.CallActivityAsync<string>("EmailUpdateFunction_Hello", emailDto);
            }
        }

        [FunctionName("EmailUpdateFunction_Process")]
        public static async Task SendEmail(
            [ActivityTrigger] UpdateEmailDto user,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
            [Inject] IQueryBus queryBus,
            ILogger log,
            CancellationToken token)
        {
            var q = new GetUpdatesEmailQuestionsQuery(user.Id);
            var d = new GetUpdatesEmailDocumentsQuery(user.Id);
            var t1 = queryBus.QueryAsync(q, token);
            var t2 = queryBus.QueryAsync(d, token);
            await Task.WhenAll(t1, t2);
            user.Questions = t1.Result;
            user.Documents = t2.Result;
            user.XQuestions = user.Questions.Count();
            user.XNewItems = user.Documents.Count();
            user.NumUpdates = user.XQuestions + user.XNewItems;
            //user.To = "ram.y@outlook.com";
            //if (user.Id == 160347)
            //{
            List<Question> questions = new List<Question>();
            List<Document> documents = new List<Document>();
            foreach (var question in user.Questions)
            {
                //TODO: fix URLs build
                questions.Add(new Question()
                {
                    QuestionUrl = $@"https://dev.spitball.co/question/{question.QuestionId}",
                    QuestionTxt = question.QuestionTxt,
                    UserPicture = $@"https://dev.spitball.co/{question.UserPicture}?&width=64&height=64&mode=crop",
                    Asker = question.Asker
                });
            }
            foreach (var document in user.Documents)
            {
                documents.Add(new Document()
                {
                    FileUrl = $@"https://dev.spitball.co/document/{new Base62(document.FileId).ToString()}",
                    FileName = document.FileName,
                    Uploader = document.Uploader,
                    ImgSource = ""
                });
            }
            var templateData = new Update(user.UserName, user.NumUpdates.ToString(),
                user.XQuestions.ToString(), user.XNewItems.ToString(), documents,
                questions, user.To);
            // var ts = JsonConvert.SerializeObject(t);



            var message = new SendGridMessage
            {
                //TODO Finish language
                Asm = new ASM { GroupId = 10926 },
                TemplateId = Language.English == Language.English ? "d-535f822f33c341d78253b97b3e35e853" : "d-6a6aead697824210b95c60ddd8d495c5"
            };
            templateData.To = user.ToEmailAddress;
            var personalization = new Personalization
            {
                TemplateData = templateData
            };


            message.Personalizations = new List<Personalization>()
            {
                personalization
            };
            message.AddCategory("updates");
            message.TrackingSettings = new TrackingSettings
            {
                Ganalytics = new Ganalytics
                {
                    UtmCampaign = "updates",
                    UtmSource = "SendGrid",
                    UtmMedium = "Email",
                    Enable = true
                }
            };
            message.AddTo(user.ToEmailAddress);
            await emailProvider.AddAsync(message, token);
        }

        [FunctionName("EmailUpdateFunction_TimerStart")]
        public static async Task HttpStart(
            [TimerTrigger("0 0 10 * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            
            string instanceId = await starter.StartNewAsync("EmailUpdateFunction", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }



        internal class Update
        {

            [JsonProperty("userName")]
            public string UserName { get; set; }
            [JsonProperty("numUpdates")]
            public string NumUpdates { get; set; }
            [JsonProperty("xQuestions")]
            public string XQuestions { get; set; }
            [JsonProperty("xNewItems")]
            public string XNewItems { get; set; }
            [JsonProperty("documents")]
            public List<Document> Documents { get; set; }
            [JsonProperty("questions")]
            public List<Question> Questions { get; set; }
            [JsonProperty("to")]
            public string To { get; set; }

            public Update(string userName, string numUpdates, string xQuestions, string xNewItems, List<Document> documents, List<Question> questions, string to)
            {

                UserName = userName;
                NumUpdates = numUpdates;
                XQuestions = xQuestions;
                XNewItems = xNewItems;
                Documents = documents;
                Questions = questions;
                To = to;
            }

        }

        internal class Question
        {
            [JsonProperty("questionUrl")]
            public string QuestionUrl { get; set; }
            [JsonProperty("userPicture")]
            public string UserPicture { get; set; }
            [JsonProperty("asker")]
            public string Asker { get; set; }
            [JsonProperty("questionTxt")]
            public string QuestionTxt { get; set; }
        }

        internal class Document
        {
            [JsonProperty("fileUrl")]
            public string FileUrl { get; set; }
            [JsonProperty("fileName")]
            public string FileName { get; set; }
            [JsonProperty("uploader")]
            public string Uploader { get; set; }
            [JsonProperty("imgSource")]
            public string ImgSource { get; set; }
        }
    }
}