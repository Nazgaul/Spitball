using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
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
            CancellationToken token)
        {
            var timeSince = DateTime.UtcNow.AddDays(-1);
            bool needToContinue;
            int page = 0;
            do
            {
                needToContinue = false;
                //TODO check assignment
                var query = new GetUpdatesEmailUsersQuery(timeSince, page++);
                var result = await context.CallActivityAsync<IEnumerable<UpdateUserEmailDto>>("EmailUpdateFunction_UserQuery", query);

                foreach (var emailDto in result)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    needToContinue = true;
                    emailDto.Since = timeSince;
                    await context.CallActivityAsync<string>("EmailUpdateFunction_Process", emailDto);
                }

            } while (needToContinue);

        }

        [FunctionName("EmailUpdateFunction_UserQuery")]
        public static async Task<IEnumerable<UpdateUserEmailDto>> GetUserQuery(
            [ActivityTrigger] GetUpdatesEmailUsersQuery query,
            [Inject] IQueryBus queryBus,
            CancellationToken token)
        {
            var result = await queryBus.QueryAsync(query, token);
            return result;
        }




        [FunctionName("EmailUpdateFunction_Process")]
        public static async Task SendEmail(
            [ActivityTrigger] UpdateUserEmailDto user,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
            [Inject] IQueryBus queryBus,
            ILogger log,
            [Inject] IUrlBuilder urlBuilder,
            [Inject] IBinarySerializer binarySerializer,
            [Inject] IDocumentDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {

            var uri = CommunicationFunction.GetHostUri();

            var questionNvc = new NameValueCollection()
            {
                ["width"] = "64",
                ["height"] = "64",
                ["mode"] = "crop"
            };


            var q = new GetUpdatesEmailByUserQuery(user.UserId, user.Since);
            var result = (await queryBus.QueryAsync(q, token)).ToList();

            var courses = result.GroupBy(g => g.Course).Select(s =>
            {
                var emailUpdates = s.Take(4).ToList();
                return new Course()
                {
                    Name = s.Key,
                    Url = urlBuilder.BuildCourseEndPoint(s.Key),
                    NeedMore = emailUpdates.Count == 4,
                    Documents = emailUpdates.OfType<DocumentUpdateEmailDto>().Select(document =>
                    {
                        var previewUri = blobProvider.GetPreviewImageLink(document.Id, 0);
                        var properties = new ImageProperties(previewUri);
                        var byteHash = binarySerializer.Serialize(properties);
                        var hash = Base64UrlTextEncoder.Encode(byteHash);


                        var uriBuilder = new UriBuilder(new Uri(uri))
                        {
                            Path = $"api/image/{hash}",
                        };
                        uriBuilder.AddQuery(questionNvc);

                        return new Document()
                        {
                            Url = urlBuilder.BuildDocumentEndPoint(document.Id),
                            Name = document.Name,
                            UserName = document.UserName,
                            DocumentPreview = uriBuilder.ToString()
                        };
                    }),
                    Questions = emailUpdates.OfType<QuestionUpdateEmailDto>().Select(question =>
                    {
                        var uriBuilder = new UriBuilder(new Uri(uri))
                        {
                            Path = $"api/image/{question.UserImage}",
                        };
                        uriBuilder.AddQuery(questionNvc);
                        return new Question()
                        {
                            QuestionUrl = urlBuilder.BuildQuestionEndPoint(question.QuestionId),
                            QuestionText = question.QuestionText,
                            UserImage = uriBuilder.ToString(),
                            UserName = question.UserName
                        };
                    })
                };
            });




            var templateData = new UpdateEmail(user.UserName, user.ToEmailAddress)
            {
                DocumentCountUpdate = result.OfType<DocumentUpdateEmailDto>().Count(),
                QuestionCountUpdate = result.OfType<QuestionUpdateEmailDto>().Count(),
                Courses = courses
            };

            var message = new SendGridMessage
            {
                Asm = new ASM { GroupId = UnsubscribeGroup.Update },
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
            //message.AddTo("ram@cloudents.com");
            await emailProvider.AddAsync(message, token);
            await emailProvider.FlushAsync(token);
        }

        [FunctionName("EmailUpdateFunction_TimerStart")]
        public static async Task TimerStart(
            [TimerTrigger("0 0 10 * * *", RunOnStartup = true)] TimerInfo myTimer,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            const string instanceName = "UpdateEmail";
            var existingInstance = await starter.GetStatusAsync(instanceName);
            if (existingInstance == null)
            {
                await starter.StartNewAsync("EmailUpdateFunction", "UpdateEmail", null);
                return;
            }
            if (existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Running)
            {
                if (existingInstance.LastUpdatedTime < DateTime.UtcNow.AddHours(-6))
                {
                    await starter.TerminateAsync(instanceName, $"Taking too long ");
                }
                else
                {
                    log.LogInformation($"{instanceName} is in status {existingInstance.RuntimeStatus}");
                    return;
                }
            }
            await starter.StartNewAsync("EmailUpdateFunction", "UpdateEmail", null);
        }



        internal class UpdateEmail
        {
            private int _questionCountUpdate;
            private int _documentCountUpdate;

            [JsonProperty("userName")]
            public string UserName { get; set; }

            [JsonProperty("numUpdates")]
            public int TotalUpdates => QuestionCountUpdate.GetValueOrDefault() + DocumentCountUpdate.GetValueOrDefault();

            [JsonProperty("xQuestions")]
            public int? QuestionCountUpdate
            {
                get => _questionCountUpdate == 0 ? (int?)null : _questionCountUpdate ;
                set => _questionCountUpdate = value.GetValueOrDefault();
            }

            [JsonProperty("xNewItems")]
            public int? DocumentCountUpdate
            {
                get => _documentCountUpdate == 0 ? (int?)null : _documentCountUpdate;
                set => _documentCountUpdate = value.GetValueOrDefault();
            }

            [JsonProperty("to")]
            public string To { get; set; }

            [JsonProperty("courseUpdates")]
            public IEnumerable<Course> Courses { get; set; }

            public UpdateEmail(string userName, string to)
            {
                UserName = userName;
                To = to;
            }

        }

        internal class Course
        {
            [JsonProperty("courseName")]
            public string Name { get; set; }
            [JsonProperty("courseUrl")]
            public string Url { get; set; }

            [JsonProperty("questions")]

            public IEnumerable<Question> Questions { get; set; }
            [JsonProperty("documents")]

            public IEnumerable<Document> Documents { get; set; }

            [JsonProperty("extraUpdates")]
            public bool NeedMore { get; set; }
        }


    }

    internal abstract class Item
    {

    }

    internal class Question : Item
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

    internal class Document : Item
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
