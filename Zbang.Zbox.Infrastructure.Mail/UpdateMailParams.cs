using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class UpdateMailParams : MailParameters
    {
        public UpdateMailParams(IEnumerable<BoxUpdate> updates, CultureInfo culture,
            string userName, int numberOfQuestions, int numberOfAnswers, int numberOfItems)
            : base(culture)
        {
            Updates = updates;
            UserName = userName;
            NoOfQuestions = numberOfQuestions;
            NoOfAnswers = numberOfAnswers;
            NoOfItems = numberOfItems;
        }
        public override string MailResover
        {
            get { return UpdateResolver; }
        }

        public IEnumerable<BoxUpdate> Updates { get; private set; }
        public string UserName { get; private set; }
        public int NoOfQuestions { get; private set; }
        public int NoOfAnswers { get; private set; }
        public int NoOfItems { get; private set; }

        public class BoxUpdate
        {
            public BoxUpdate(string boxName, IEnumerable<BoxUpdateDetails> updates, string boxUrl, int numberOfExtraUpdates)
            {
                BoxName = boxName;
                Updates = updates;
                Url = boxUrl;
                ExtraUpdatesCount = numberOfExtraUpdates;
            }
            public string BoxName { get; private set; }
            public string Url { get; private set; }
            public int ExtraUpdatesCount { get; private set; }
            public IEnumerable<BoxUpdateDetails> Updates { get; private set; }

        }
        public abstract class BoxUpdateDetails
        {
            protected BoxUpdateDetails(string url, string picture, long userId)
            {
                Url = url;
                Picture = picture;
                UserId = userId;
            }
            
            public long UserId { get; private set; }
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
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Item"));
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
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Question"));
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
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Answer"));
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
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Discussion"));
                sb.Replace("{USERNAME}", User);
                sb.Replace("{ANSWER_URL}", Url);
                sb.Replace("{BOX_PICTURE}", Picture);
                sb.Replace("{COMMENT-TXT}", Text);

                return sb.ToString();
            }
        }

        public class UserJoin : BoxUpdateDetails
        {
            public UserJoin(string name, string picture, string boxName, string url, long userId)
                : base(url, picture, userId)
            {
                Name = name;
                BoxName = boxName;
            }
            public string Name { get; set; }
            public string BoxName { get; private set; }


            public override string BuildMailLine(CultureInfo culture)
            {
                var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(culture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.UpdatesEmail.Member"));
                sb.Replace("{MEMBER-NAME}", Name);
                sb.Replace("{MEMBER_URL}", Url);
                sb.Replace("{MEMBER_PICTURE}", Picture);
                sb.Replace("{BOX-NAME}", BoxName);

                return sb.ToString();
            }
        }


    }
}