using System.Text;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class SpamGunMail : MailBuilder
    {
        private readonly SpamGunMailParams _parameters;
        public SpamGunMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as SpamGunMailParams;
        }

        public override string GenerateMail()
        {
            var html = new StringBuilder(_parameters.HtmlBody);
            html.Replace("{name}", _parameters.Name);
            html.Replace("{body}", _parameters.Body.Replace("\n", "<br>"));
            //html.Replace("{uni_Url}", _parameters.UniversityUrl);
            return html.ToString();
        }

        public override string AddSubject()
        {
            return _parameters.Subject;
        }

        public override string AddCategory()
        {
            return _parameters.Category;
        }
    }

    //public class GreekPartnerMail : MailBuilder
    //{
    //    private readonly GreekPartnerMailParams _parameters;
    //    public GreekPartnerMail(MailParameters parameters) : base(parameters)
    //    {
    //        _parameters = parameters as GreekPartnerMailParams;
    //    }

    //    public override string GenerateMail()
    //    {
    //        var html = LoadMailTempate.LoadMailFromContentWithDot(new CultureInfo("en-US"), "Zbang.Zbox.Infrastructure.Mail.MailTemplate.GreekHouseTemplate");
    //        html.Replace("{name}", _parameters.Name);
    //        html.Replace("{body}", _parameters.Body.Replace("\n", "<br>"));
    //        html.Replace("{uni_Url}", _parameters.UniversityUrl);
    //        html.Replace("{School}", _parameters.School);
    //        html.Replace("{Chapter}", _parameters.Chapter);
    //        return html.ToString();
    //    }

    //    public override string AddSubject()
    //    {
    //        return _parameters.Subject.Replace("{Chapter}", _parameters.Chapter);
    //    }

    //    public override string AddCategory()
    //    {
    //        return _parameters.Category;
    //    }
    //}
}