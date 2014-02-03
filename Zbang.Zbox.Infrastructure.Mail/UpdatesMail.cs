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
        //{Updates}
        public void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters)
        {
            Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);


            var updateParams = parameters as UpdateMailParams;
            Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(updateParams, "updateParams");

            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Updates");

            var cube = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesBoxCube");
            var line = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesLine");

            var sb = new StringBuilder();
            foreach (var boxUpdate in updateParams.Updates)
            {
                sb.Append(generateBoxCube(boxUpdate, message, cube, line));
            }


            //message.Text = textBody;
            message.Subject = Subject;
            message.Html = message.Html.Replace("{Updates}", sb.ToString());
            // message.AddSubVal("{Updates}", new List<string> { sb.ToString() });
            //message.AddSubVal("{USER-NAME}", new List<string> { "Ram" });
        }

        private string generateBoxCube(UpdateMailParams.BoxUpdate boxUpdate, SendGridMail.ISendGrid message, string cube, string line)
        {
            // var randomBoxStringReplacement = Guid.NewGuid().ToString();
            cube = cube.Replace("{BOX-NAME}", boxUpdate.BoxName);

            //message.AddSubVal(randomBoxStringReplacement, new List<string> { boxUpdate.BoxName });
            StringBuilder sb = new StringBuilder();
            foreach (var update in boxUpdate.Updates)
            {
                sb.Append(generateLineUpdate(update, line));
            }
            cube = cube.Replace("{lineOfUpdates}", sb.ToString());
            return cube;
        }

        private string generateLineUpdate(UpdateMailParams.BoxUpdateDetails update, string line)
        {
            return line.Replace("{USER-NAME}", update.UserName).Replace("{ACTION-ELEM}", update.ActionElement)
                   .Replace("{ACTION-TEXT}", update.ActionText.GetEnumDescription()).Replace("{ACTION-URL}", update.ActionUrl);
        }
    }
}
