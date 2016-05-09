using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Web.Http;
using Zbang.Zbox.Infrastructure;

namespace Zbang.Cloudents.MobileApp.Filters
{
    public class EmailVerifyAttribute : ValidationAttribute
    {
        
        private readonly IEmailVerification m_EmailVerification;
        
        public IEmailVerification EmailVerification { get; set; }

        public EmailVerifyAttribute()
        {

            m_EmailVerification = new EmailVerification();
        }
        
        public override bool IsValid(object value)
        {
            
            return EmailVerification.VerifyEmail(value.ToString());
        }
    }
}