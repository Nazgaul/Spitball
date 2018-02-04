using System.Globalization;
using System.Text;
using System.Threading;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class UpdatesMail : MailBuilder
    {
        private const string Category = "Update";
        private const string Subject = "Spitball Update";

        private readonly UpdateMailParams _parameters;

        public UpdatesMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as UpdateMailParams;
        }

        public override string GenerateMail()
        {
            Thread.CurrentThread.CurrentUICulture = _parameters.UserCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_parameters.UserCulture.Name);

            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Updates1");

            var cube = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.UpdatesList1");

            var sb = new StringBuilder();
            foreach (var boxUpdate in _parameters.Updates)
            {
                sb.Append(GenerateBoxCube(boxUpdate, _parameters.UserCulture, cube));
            }

            //message.Text = textBody;

            html = html.Replace("{UPDATES}", sb.ToString());
            html = html.Replace("{USERNAME}", _parameters.UserName);
            html = html.Replace("{NUM-UPDATES}", AggregateUpdates(_parameters.NoOfUpdates));
            html = html.Replace("{X-ANSWERS}", AggregateAnswers(_parameters.NoOfAnswers));
            html = html.Replace("{X-QUESTIONS}", AggregateQuestion(_parameters.NoOfQuestions));
            html = html.Replace("{X-NEW-ITEMS}", AggregateItems(_parameters.NoOfItems));

            var spaceInGmail = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            html = html.Replace(spaceInGmail, string.Empty);
            return html;
        }

        public override string AddSubject()
        {
            return Subject;
        }

        public override string AddCategory()
        {
            return Category;
        }

        private static string AggregateUpdates(int numOfUpdates)
        {
            var str = AggregateWithString(numOfUpdates, EmailResource.Update, EmailResource.Updates);
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            return str.Remove(str.Length - 1, 1);
        }

        private static string AggregateAnswers(int numOfAnswers)
        {
            return AggregateWithString(numOfAnswers, EmailResource.answer, EmailResource.answers);
        }

        private static string AggregateQuestion(int numOfQuestion)
        {
            return AggregateWithString(numOfQuestion, EmailResource.question, EmailResource.questions);
        }

        private static string AggregateItems(int numOfItems)
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

        private static string GenerateBoxCube(UpdateMailParams.BoxUpdate boxUpdate, CultureInfo culture, string cube)
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
