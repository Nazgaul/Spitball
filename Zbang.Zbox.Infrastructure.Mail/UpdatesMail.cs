﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class UpdatesMail : IMailBuilder
    {
        const string Category = "Update";
        const string Subject = "Cloudents Update";

        // private string cube, moreTemplate;
        //answerTemplate, itemTemplate, memberTemplate, , questionTemplate;

        public UpdatesMail()
        {

        }

        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);


            var updateParams = parameters as UpdateMailParams;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(updateParams, "updateParams");

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Updates");

            var cube = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.UpdatesList");
            //answerTemplate = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Answer");
            ////itemTemplate = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Item");
            //memberTemplate = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Member");

            //questionTemplate = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Question");


            var sb = new StringBuilder();
            foreach (var boxUpdate in updateParams.Updates)
            {
                sb.Append(generateBoxCube(boxUpdate, parameters.UserCulture, cube));
            }


            //message.Text = textBody;
            message.Subject = Subject;

            message.Html = message.Html.Replace("{UPDATES}", sb.ToString());
            message.Html = message.Html.Replace("{USERNAME}", updateParams.UserName);
            message.Html = message.Html.Replace("{NUM-UPDATES}", (updateParams.NoOfAnswers + updateParams.NoOfItems + updateParams.NoOfQuestions + updateParams.NoOfUsers).ToString());
            message.Html = message.Html.Replace("{X-ANSWERS}", AggregateAnswers(updateParams.NoOfAnswers));
            message.Html = message.Html.Replace("{X-QUESTIONS}", AggregateQuestion(updateParams.NoOfQuestions));
            message.Html = message.Html.Replace("{X-NEW-ITEMS}", AggregateItems(updateParams.NoOfItems));
            message.Html = message.Html.Replace("{X-NEW-USERS}", AggregateUsers(updateParams.NoOfUsers));

            // message.AddSubVal("{Updates}", new List<string> { sb.ToString() });
            //message.AddSubVal("{USER-NAME}", new List<string> { "Ram" });
        }

        private string AggregateUsers(int numOfUsers)
        {
            return AggreateWithString(numOfUsers, EmailResource.user, EmailResource.users);
        }

        private string AggregateAnswers(int numOfAnswers)
        {
            return AggreateWithString(numOfAnswers, EmailResource.answer, EmailResource.answers);
        }
        private string AggregateQuestion(int numOfQuestion)
        {
            return AggreateWithString(numOfQuestion, EmailResource.question, EmailResource.questions);
        }
        private string AggregateItems(int numOfItems)
        {
            return AggreateWithString(numOfItems, EmailResource.item, EmailResource.items);
        }
        private string AggreateWithString(int number, string single, string many)
        {
            if (number == 1)
            {
                return string.Format("1 {0},", single);
            }
            if (number == 0)
            {
                return string.Empty;
            }
            return string.Format("{0} {1},", number, many);
        }

        private string generateBoxCube(UpdateMailParams.BoxUpdate boxUpdate, CultureInfo culture, string cube)
        {
            cube = cube.Replace("{BOX-NAME}", boxUpdate.BoxName);

            StringBuilder sb = new StringBuilder();
            foreach (var update in boxUpdate.Updates)
            {
                sb.Append(update.BuildMailLine(culture));
            }
            if (boxUpdate.ExtraUpdatesCount > 0)
            {
                var moreTemplate = LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.More");
                moreTemplate = moreTemplate.Replace("{NUM-MORE}", boxUpdate.ExtraUpdatesCount.ToString());
                moreTemplate = moreTemplate.Replace("{BOX-URL}", boxUpdate.Url);
                sb.Append(moreTemplate);
            }
            cube = cube.Replace("{BOX_UPDATES}", sb.ToString());
            return cube;
        }


    }
}
