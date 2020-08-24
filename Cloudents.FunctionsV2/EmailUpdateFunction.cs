using Cloudents.Core.DTOs.Email;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.Core.Message.Email;
using Cloudents.FunctionsV2.Services;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using Course = Cloudents.FunctionsV2.Models.Course;
using Document = Cloudents.FunctionsV2.Models.Document;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class EmailUpdateFunction
    {
        public const string HebrewTemplateId = "d-6a6aead697824210b95c60ddd8d495c5";
        public const string EnglishTemplateId = "d-535f822f33c341d78253b97b3e35e853";

        [FunctionName("EmailUpdateFunction")]
        public static async Task RunOrchestratorAsync(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            CancellationToken token)
        {
            var timeSince = DateTime.UtcNow.AddDays(-1);
            bool needToContinue;
            var page = 0;
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
        public static async Task<IEnumerable<UpdateUserEmailDto>> GetUserQueryAsync(
            [ActivityTrigger] GetUpdatesEmailUsersQuery query,
            [Inject] IQueryBus queryBus,
            CancellationToken token)
        {
            var result = await queryBus.QueryAsync(query, token);
            return result;
        }




        [FunctionName("EmailUpdateFunction_Process")]
        public static async Task SendEmailAsync(
            [ActivityTrigger] UpdateUserEmailDto user,
            [SendGrid(ApiKey = "SendgridKey", From = "Spitball <no-reply@spitball.co>")] IAsyncCollector<SendGridMessage> emailProvider,
            [Inject] IQueryBus queryBus,
            [Inject] IUrlBuilder urlBuilder,
            [Inject] IDataProtectionService dataProtectService,
            [Inject] IHostUriService hostUriService,
            CancellationToken token)
        {

            var code = dataProtectService.ProtectData(user.UserId.ToString(), DateTimeOffset.UtcNow.AddDays(3));
            var uri = hostUriService.GetHostUri();

            var questionNvc = new NameValueCollection()
            {
                ["width"] = "86",
                ["height"] = "96",
                ["mode"] = "crop"
            };

            var q = new GetUpdatesEmailByUserQuery(user.UserId, user.Since);
            var result = (await queryBus.QueryAsync(q, token)).ToList();
            if (result.Count == 0)
            {
                return;
            }
            var courses = result.GroupBy(g => g.Course).Take(3).Select(s =>
            {
                var emailUpdates = s.Take(4).ToList();
                return new Course()
                {
                    Name = s.Key,
                    Url = urlBuilder.BuildCourseEndPoint(s.Key),
                    NeedMore = emailUpdates.Count == 4,
                    Documents = emailUpdates.OfType<DocumentUpdateEmailDto>().Select(document =>
                    {
                        var uriBuilder = new UriBuilder(uri)
                        {
                            Path = $"api/image/document/{document.Id}",
                        };
                        uriBuilder.AddQuery(questionNvc);

                        return new Document()
                        {
                            Url = urlBuilder.BuildDocumentEndPoint(document.Id, new { token = code }),
                            Name = document.Name,
                            UserName = document.UserName,
                            DocumentPreview = uriBuilder.ToString(),
                            UserImage = BuildUserImage(document.UserId, document.UserImage, document.UserName, hostUriService)
                        };
                    }),
                    //Questions = emailUpdates.OfType<QuestionUpdateEmailDto>().Select(question => new Question()
                    //{
                    //    QuestionUrl = urlBuilder.BuildQuestionEndPoint(question.QuestionId, new { token = code }),
                    //    QuestionText = question.QuestionText,
                    //    UserImage = BuildUserImage(question.UserId, question.UserImage, question.UserName, hostUriService),
                    //    UserName = question.UserName,
                    //    AnswerText = question.AnswerText
                    //})
                };
            });

            var templateData = new UpdateEmail(user.UserName, user.ToEmailAddress, user.Language.TextInfo.IsRightToLeft)
            {
                DocumentCountUpdate = result.OfType<DocumentUpdateEmailDto>().Count(),
                //QuestionCountUpdate = result.OfType<QuestionUpdateEmailDto>().Count(),
                Courses = courses
            };

            var message = new SendGridMessage
            {
                Asm = new ASM { GroupId = UnsubscribeGroup.Update },
                TemplateId = Equals(user.Language, Language.Hebrew.Info)
                    ? HebrewTemplateId : EnglishTemplateId
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
            await emailProvider.FlushAsync(token);
        }

        private static string BuildUserImage(long id, string? image, string name, IHostUriService hostUriService)
        {

            var uri = hostUriService.GetHostUri();
            var uriBuilderImage = new UriBuilder(uri)
            {
                Path = $"api/image/user/{id}/{image ?? name}"
            };
            var userImageNvc = new NameValueCollection()
            {
                ["width"] = "34",
                ["height"] = "34",
                ["mode"] = "crop"
            };
            uriBuilderImage.AddQuery(userImageNvc);
            return uriBuilderImage.ToString();
        }



        [FunctionName("EmailUpdateFunction_TimerStart")]
        public static async Task TimerStartAsync(
            [TimerTrigger("0 0 8 * * *")] TimerInfo myTimer,
            [DurableClient]IDurableOrchestrationClient starter,
            ILogger log)
        {
            const string instanceName = "UpdateEmail";
            var existingInstance = await starter.GetStatusAsync(instanceName);
            if (existingInstance == null)
            {
                await starter.StartNewAsync("EmailUpdateFunction", "UpdateEmail");
                return;
            }

            var types = new[] { OrchestrationRuntimeStatus.Running, OrchestrationRuntimeStatus.Pending };
            if (types.Contains(existingInstance.RuntimeStatus))
            {
                if (existingInstance.LastUpdatedTime < DateTime.UtcNow.AddHours(-6))
                {
                    await starter.TerminateAsync(instanceName, "Taking too long ");
                }
                else
                {
                    log.LogInformation($"{instanceName} is in status {existingInstance.RuntimeStatus}");
                    return;
                }
            }


            await starter.StartNewAsync("EmailUpdateFunction", "UpdateEmail");
        }









    }

    //public class Question : Item
    //{
    //    [JsonProperty("questionUrl")]
    //    public string QuestionUrl { get; set; }
    //    [JsonProperty("userPicture")]
    //    public string UserImage { get; set; }
    //    [JsonProperty("asker")]
    //    public string UserName { get; set; }
    //    [JsonProperty("questionTxt")]
    //    public string QuestionText { get; set; }
    //    [JsonProperty("answerText")]
    //    public string AnswerText { get; set; } //NEW
    //}
}
