using Cobisi.EmailVerify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class EmailVerification : IEmailVerfication
    {
      //  VerificationEngine m_Engine;
        public EmailVerification()
        {
            try
            {
                LicensingManager.SetLicenseKey("FHbrz2C/8XTEPkmsVzPzPuYvZ2XNoMDfMEdJKJdvGlwPpkAgNwQMVT+Ae1ZSY8QbQpm+7g==");
               
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On check email ctor", ex);
            }
        }

        public async Task<bool> VerifyEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            try
            {
                using (var m_Engine = new VerificationEngine())
                {
                    m_Engine.DefaultSettings.DnsServers.Clear();
                    m_Engine.DefaultSettings.DnsServers.Add(IPAddress.Parse("8.8.8.8"));
                    m_Engine.DefaultSettings.DnsServers.Add(IPAddress.Parse("8.8.4.4"));

                    var verificationEmail = new Verification(email);

                    await m_Engine.RunAsync(verificationEmail, VerificationLevel.Smtp);

                    return verificationEmail.State.Result.LastStatus == VerificationStatus.Success;
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On check email email : " + email, ex);
                return true;
            }
        }

    }
}