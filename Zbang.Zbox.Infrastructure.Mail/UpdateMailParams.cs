using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class UpdateMailParams : MailParameters
    {
        public UpdateMailParams(IEnumerable<BoxUpdate> updates, CultureInfo culture,
            string userName, int numberOfQuestions, int numberOfAnswers, int numberOfItems, int noOfUpdates)
            : base(culture)
        {
            Updates = updates;
            UserName = userName;
            NoOfQuestions = numberOfQuestions;
            NoOfAnswers = numberOfAnswers;
            NoOfItems = numberOfItems;
            NoOfUpdates = noOfUpdates;
        }

        public override string MailResolver => UpdateResolver;

        public IEnumerable<BoxUpdate> Updates { get; }
        public string UserName { get; }
        public int NoOfQuestions { get; }
        public int NoOfAnswers { get; }
        public int NoOfItems { get; }

        public int NoOfUpdates { get; }

        public class BoxUpdate
        {
            public BoxUpdate(string boxName, IEnumerable<BoxUpdateDetails> updates, string boxUrl, int numberOfExtraUpdates)
            {
                BoxName = boxName;
                Updates = updates;
                Url = boxUrl;
                ExtraUpdatesCount = numberOfExtraUpdates;
            }

            public string BoxName { get; }
            public string Url { get; }
            public int ExtraUpdatesCount { get; }
            public IEnumerable<BoxUpdateDetails> Updates { get; }
        }

        public abstract class BoxUpdateDetails
        {
            protected BoxUpdateDetails(string url, string picture, long userId)
            {
                Url = url;
                Picture = picture;
                UserId = userId;
            }

            public long UserId { get; }
            public string Url { get; set; }
            public string Picture { get; set; }

            public abstract string BuildMailLine(CultureInfo culture);
        }

        public class ItemUpdate : BoxUpdateDetails
        {
            public ItemUpdate(string name, string picture, string ownerName, string url, long userId)
                : base(url, picture, userId)
            {
                Name = name;
                OwnerName = ownerName;
            }

            public string Name { get; set; }
            public string OwnerName { get; set; }

            public override string BuildMailLine(CultureInfo culture)
            {
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Item1"));
                sb.Replace("{FILE_URL}", Url);
                sb.Replace("{IMG-SOURCE}", Picture);
                sb.Replace("{FILE-NAME}", Name);
                sb.Replace("{UPLOADER}", OwnerName);
                return sb.ToString();
            }
        }

        public class QuestionUpdate : BoxUpdateDetails
        {
            public QuestionUpdate(string user, string text, string picture, string url, long userId)
                : base(url, picture, userId)
            {
                User = user;
                Text = text;
            }

            public string User { get; set; }
            public string Text { get; set; }

            public override string BuildMailLine(CultureInfo culture)
            {
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Question1"));
                sb.Replace("{ASKER}", User);
                sb.Replace("{QUESTION_URL}", Url);
                sb.Replace("{BOX_PICTURE}", Picture);
                sb.Replace("{ANSWER-URL}", Url);
                sb.Replace("{QUESTION-TXT}", Text);

                return sb.ToString();
            }
        }

        public class AnswerUpdate : BoxUpdateDetails
        {
            public AnswerUpdate(string user, string text, string picture, string url, long userId)
                : base(url, picture, userId)
            {
                User = user;
                Text = text;
            }

            public string User { get; set; }
            public string Text { get; set; }

            public override string BuildMailLine(CultureInfo culture)
            {
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Answer1"));
                sb.Replace("{ANSWERER}", User);
                sb.Replace("{ANSWER_URL}", Url);
                sb.Replace("{BOX_PICTURE}", Picture);
                sb.Replace("{ANSWER-TXT}", Text);

                return sb.ToString();
            }
        }

        public class DiscussionUpdate : BoxUpdateDetails
        {
            public DiscussionUpdate(string user, string text, string picture, string url, long userId)
                : base(url, picture, userId)
            {
                User = user;
                Text = text;
            }

            public string User { get; set; }
            public string Text { get; set; }

            public override string BuildMailLine(CultureInfo culture)
            {
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Discussion1"));
                sb.Replace("{USERNAME}", User);
                sb.Replace("{ANSWER_URL}", Url);
                sb.Replace("{BOX_PICTURE}", Picture);
                sb.Replace("{COMMENT-TXT}", Text);

                return sb.ToString();
            }
        }
    }
}