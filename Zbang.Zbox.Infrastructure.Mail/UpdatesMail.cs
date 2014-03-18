using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            message.Html = message.Html.Replace("{NUM-UPDATES}", (updateParams.NoOfAnswers + updateParams.NoOfItems + updateParams.NoOfQuestions).ToString());
            message.Html = message.Html.Replace("{X-ANSWERS}", updateParams.NoOfAnswers.ToString());
            message.Html = message.Html.Replace("{X-QUESTIONS}", updateParams.NoOfQuestions.ToString());
            message.Html = message.Html.Replace("{X-NEW-ITEMS}", updateParams.NoOfItems.ToString());
            // message.AddSubVal("{Updates}", new List<string> { sb.ToString() });
            //message.AddSubVal("{USER-NAME}", new List<string> { "Ram" });
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
