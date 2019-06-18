using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Query;
using Cloudents.Query.Email;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public class UpdatesEmailOperation : ISystemOperation<UpdatesEmailMessage>
    {
        private readonly IQueryBus _queryBus;
        private readonly IDataProtectionProvider _dataProtectProvider;
        private readonly IUrlBuilder _urlBuilder;
        const string category = "updates";
        public UpdatesEmailOperation(IQueryBus queryBus, IDataProtectionProvider dataProtectProvider, IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _dataProtectProvider = dataProtectProvider;
            _urlBuilder = urlBuilder;
        }

        private static async Task BuildEmail(string toAddress, Language language, IBinder binder,
           Update templateData,
           string category,
           CancellationToken token)
        {
            var emailProvider = await binder.BindAsync<IAsyncCollector<SendGridMessage>>(new SendGridAttribute()
            {
                ApiKey = "SendgridKey",
                From = "Spitball <no-reply @spitball.co>"
            }, token);


            var message = new SendGridMessage
            {
                Asm = new ASM { GroupId = 10926 },
                TemplateId = language == Language.English ? "d-535f822f33c341d78253b97b3e35e853" : "d-535f822f33c341d78253b97b3e35e853"
            };
            templateData.To = toAddress;
            var personalization = new Personalization
            {
                TemplateData = templateData
            };


            message.Personalizations = new List<Personalization>()
            {
                personalization
            };
            message.AddCategory(category);
            message.TrackingSettings = new TrackingSettings
            {
                Ganalytics = new Ganalytics
                {
                    UtmCampaign = category,
                    UtmSource = "SendGrid",
                    UtmMedium = "Email",
                    Enable = true
                }
            };
            message.AddTo(toAddress);
            await emailProvider.AddAsync(message, token);
        }

        public async Task DoOperationAsync(UpdatesEmailMessage msg, IBinder binder, CancellationToken token)
        {
            var query = new GetUpdatesEmailUsersQuery();
            var users = await _queryBus.QueryAsync(query, token);

            foreach (var user in users)
            {
                var q = new GetUpdatesEmailQuestionsQuery(user.Id);
                var d = new GetUpdatesEmailDocumentsQuery(user.Id);
                var t1 = _queryBus.QueryAsync(q, token);
                var t2 = _queryBus.QueryAsync(d, token);
                await Task.WhenAll(t1, t2);
                user.Questions = t1.Result;
                user.Documents = t2.Result; 
                user.XQuestions = user.Questions.Count();
                user.XNewItems = user.Documents.Count();
                user.NumUpdates = user.XQuestions + user.XNewItems;
                //user.To = "ram.y@outlook.com";
                if (user.Id == 160347)
                {
                    List<Question> questions = new List<Question>();
                    List<Document> documents = new List<Document>();
                    foreach (var question in user.Questions)
                    {
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
                    var t = new Update(user.UserName, user.NumUpdates.ToString(), user.XQuestions.ToString(), user.XNewItems.ToString(), documents,
                        questions, user.To);
                   // var ts = JsonConvert.SerializeObject(t);
                    await BuildEmail(t.To, "en", binder,
                          t,
                          category,
                          token);
                }

            }
           
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
