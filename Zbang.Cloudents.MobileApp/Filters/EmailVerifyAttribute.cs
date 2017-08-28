using System;
using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.MobileApp.Filters
{
    //Wont work since we not in the domain of spitball
    public class EmailVerifyAttribute : ValidationAttribute, IDisposable
    {
        private readonly EmailVerification m_EmailVerification;
        
       // public IEmailVerification EmailVerification { get; set; }

       public EmailVerifyAttribute()
        {
            m_EmailVerification = new EmailVerification(new Logger());
        }
        
        public override bool IsValid(object value)
        {
            return m_EmailVerification.VerifyEmail(value.ToString());
        }

        public void Dispose()
        {
            m_EmailVerification.Dispose();
        }
    }
}