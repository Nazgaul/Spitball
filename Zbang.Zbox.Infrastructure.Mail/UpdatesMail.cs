using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using SendGrid;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class UpdatesMail : IMailBuilder
    {
        const string Category = "Update";
        const string Subject = "Spitball Update";

       
        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);


            var updateParams = parameters as UpdateMailParams;
            if (updateParams == null)
            {
                throw new NullReferenceException("updateParams");
            }

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Updates1");

            var cube = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.UpdatesList1");

            var sb = new StringBuilder();
            foreach (var boxUpdate in updateParams.Updates)
            {
                sb.Append(GenerateBoxCube(boxUpdate, parameters.UserCulture, cube));
            }


            //message.Text = textBody;
            

            message.Html = message.Html.Replace("{UPDATES}", sb.ToString());
            message.Html = message.Html.Replace("{USERNAME}", updateParams.UserName);
            message.Html = message.Html.Replace("{NUM-UPDATES}", updateParams.NoOfUpdates.ToString(CultureInfo.InvariantCulture));
            message.Html = message.Html.Replace("{X-ANSWERS}", AggregateAnswers(updateParams.NoOfAnswers));
            message.Html = message.Html.Replace("{X-QUESTIONS}", AggregateQuestion(updateParams.NoOfQuestions));
            message.Html = message.Html.Replace("{X-NEW-ITEMS}", AggregateItems(updateParams.NoOfItems));

            var spaceInGmail = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            message.Html = message.Html.Replace(spaceInGmail, string.Empty);
            message.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: "updateEmail");
        }

        public void AddSubject(ISendGrid message)
        {
            message.Subject = Subject;
        }

        private string AggregateAnswers(int numOfAnswers)
        {
            return AggregateWithString(numOfAnswers, EmailResource.answer, EmailResource.answers);
        }
        private string AggregateQuestion(int numOfQuestion)
        {
            return AggregateWithString(numOfQuestion, EmailResource.question, EmailResource.questions);
        }
        private string AggregateItems(int numOfItems)
        {
            return AggregateWithString(numOfItems, EmailResource.item, EmailResource.items);
        }
        private static string AggregateWithString(int number, string single, string many)
        {
            if (number == 1)
            {
                return $"1 {single},";
            }
            if (number == 0)
            {
                return string.Empty;
            }
            return $"{number} {many},";
        }

        private string GenerateBoxCube(UpdateMailParams.BoxUpdate boxUpdate, CultureInfo culture, string cube)
        {
            cube = cube.Replace("{BOX-NAME}", boxUpdate.BoxName);
            cube = cube.Replace("{BOX-URL}", boxUpdate.Url);

            var sb = new StringBuilder();
            foreach (var update in boxUpdate.Updates)
            {
              

                sb.Append(update.BuildMailLine(culture));
            }
            if (boxUpdate.ExtraUpdatesCount > 0)
            {
                var moreTemplate = LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.More1");
                moreTemplate = moreTemplate.Replace("{NUM-MORE}", boxUpdate.ExtraUpdatesCount.ToString(CultureInfo.InvariantCulture));
                moreTemplate = moreTemplate.Replace("{BOX-URL}", boxUpdate.Url);
                sb.Append(moreTemplate);
            }
            cube = cube.Replace("{BOX_UPDATES}", sb.ToString());
            return cube;
        }


    }
}
