using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Mail;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class EmailVerifyAttribute : ValidationAttribute
    {
        private readonly IEmailVerification m_EmailVerification;

        public EmailVerifyAttribute()
        {
            m_EmailVerification = DependencyResolver.Current.GetService<IEmailVerification>();
        }
        public override bool IsValid(object value)
        {
            return m_EmailVerification.VerifyEmail(value.ToString());
        }
    }
}