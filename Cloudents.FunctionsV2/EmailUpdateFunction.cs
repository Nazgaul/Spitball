using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
            var timeSince = DateTime.UtcNow.AddDays(-30);
            bool needToContinue;
            int page = 0;
            do
            {
                needToContinue = false;
                //TODO check assignment
                var query = new GetUpdatesEmailUsersQuery(timeSince, page++);
                var result = await context.CallActivityAsync<UpdateEmailDto>("EmailUpdateFunction_UserQuery", query);

                foreach (var emailDto in result)
                {
                    needToContinue = true;
                    emailDto.Since = timeSince;
                    await context.CallActivityAsync<string>("EmailUpdateFunction_Hello", emailDto);
                }

            } while (needToContinue);
           
        }

        [FunctionName("EmailUpdateFunction_UserQuery")]
        public static async Task<IEnumerable<UpdateEmailDto>> GetUserQuery(
            [ActivityTrigger] GetUpdatesEmailUsersQuery query,
            [Inject] IQueryBus queryBus,
            CancellationToken token)
        {
            var result = await queryBus.QueryAsync(query, token);
            return result;
        }

        [FunctionName("EmailUpdateFunction_Process")]
        public static async Task SendEmail(
            [ActivityTrigger] UpdateEmailDto user,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
            [Inject] IQueryBus queryBus,
            ILogger log,
            [Inject] IUrlBuilder urlBuilder,
            [Inject] IBinarySerializer binarySerializer,
            [Inject] IDocumentDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var q = new GetUpdatesEmailByUserQuery(user.UserId, user.Since);
            var result = await queryBus.QueryAsync(q, token);

            var uri = CommunicationFunction.GetHostUri();

            var questionNvc = new NameValueCollection()
            {
                ["width"] = "64",
                ["height"] = "64",
                ["mode"] = "crop"
            };

            var questions = result.Item1.Select(question =>
            {

                var uriBuilder = new UriBuilder(new Uri(uri))
                {
                    Path = $"/image/{question.UserImage}",
                };
                uriBuilder.AddQuery(questionNvc);
                return new Question()
                {
                    QuestionUrl = urlBuilder.BuildQuestionEndPoint(question.QuestionId),
                    QuestionText = question.QuestionText,
                    UserImage = uriBuilder.ToString(),
                    UserName = question.UserName
                };
            });
            var documents = result.Item2.Select(document =>
            {
                var previewUri = blobProvider.GetPreviewImageLink(document.Id, 0);
                var properties = new ImageProperties(previewUri);
                var byteHash = binarySerializer.Serialize(properties);
                var hash = Base64UrlTextEncoder.Encode(byteHash);


                var uriBuilder = new UriBuilder(new Uri(uri))
                {
                    Path = $"/image/{hash}",
                };
                uriBuilder.AddQuery(questionNvc);

                return new Document()
                {
                    Url = urlBuilder.BuildDocumentEndPoint(document.Id),
                    Name = document.Name,
                    UserName = document.UserName,
                    DocumentPreview = uriBuilder.ToString()
                };
            });
           
            var templateData = new UpdateEmail(user.UserName,  documents, questions, user.ToEmailAddress);

            var message = new SendGridMessage
            {
                //TODO Finish language
                Asm = new ASM { GroupId = 10926 },
                TemplateId = Equals(user.Language, Language.English.Info) ? "d-535f822f33c341d78253b97b3e35e853" : "d-6a6aead697824210b95c60ddd8d495c5"
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



        internal class UpdateEmail
        {

            [JsonProperty("userName")]
            public string UserName { get; set; }

            [JsonProperty("numUpdates")]
            public int TotalUpdates => QuestionCountUpdate + DocumentCountUpdate;

            [JsonProperty("xQuestions")]
            public int QuestionCountUpdate => Questions.Count;

            [JsonProperty("xNewItems")]
            public int DocumentCountUpdate => Documents.Count;
            [JsonProperty("documents")]
            public List<Document> Documents { get; set; }
            [JsonProperty("questions")]
            public List<Question> Questions { get; set; }
            [JsonProperty("to")]
            public string To { get; set; }

            public UpdateEmail(string userName, IEnumerable<Document> documents, IEnumerable<Question> questions, string to)
            {
                UserName = userName;
                Documents = documents.ToList();
                Questions = questions.ToList();
                To = to;
            }

        }

        internal class Question
        {
            [JsonProperty("questionUrl")]
            public string QuestionUrl { get; set; }
            [JsonProperty("userPicture")]
            public string UserImage { get; set; }
            [JsonProperty("asker")]
            public string UserName { get; set; }
            [JsonProperty("questionTxt")]
            public string QuestionText { get; set; }
        }

        internal class Document
        {
            [JsonProperty("fileUrl")]
            public string Url { get; set; }
            [JsonProperty("fileName")]
            public string Name { get; set; }
            [JsonProperty("uploader")]
            public string UserName { get; set; }
            [JsonProperty("imgSource")]
            public string DocumentPreview { get; set; }
        }
    }
}