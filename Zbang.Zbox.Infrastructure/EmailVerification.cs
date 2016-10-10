using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cobisi.EmailVerify;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure
{
    public sealed class EmailVerification : IEmailVerification, IDisposable
    {
        private readonly VerificationEngine m_Engine;
        private readonly VerificationLevel m_VerificationLevel = VerificationLevel.Dns;
        private const string EmailRegex2 = @"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-z0-9]{1}[a-z0-9\-]{0,62}[a-z0-9]{1})|[a-z])\.)+[a-z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$";

        public EmailVerification()
        {
            try
            {
                //FHbrz2C/8XTEPkmsVzPzPuYvZ2XNoMDfMEdJKJdvGlwPpkAgNwQMVT+Ae1ZSY8QbQpm+7g==
                LicensingManager.SetLicenseKey("FHbrz2C/8XTEPkmsVzPzPuYvZ2XNoMDfMEdJKJdvGlwPpkAgNwQMVT+Ae1ZSY8QbQpm+7g==");
                m_Engine = new VerificationEngine();
                m_Engine.DefaultSettings.DnsServers.Clear();
                m_Engine.DefaultSettings.DnsServers.Add(IPAddress.Parse("8.8.8.8"));
                m_Engine.DefaultSettings.DnsServers.Add(IPAddress.Parse("8.8.4.4"));
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
            if (m_Engine == null)
            {
                return Regex.IsMatch(email, EmailRegex2, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(10));
            }
            try
            {
                var verificationEmail = new Verification(email);
                await m_Engine.RunAsync(verificationEmail, m_VerificationLevel);
                return verificationEmail.State.Result.LastStatus == VerificationStatus.Success;
            }
            catch (Exception ex)
            {

                TraceLog.WriteError("On check email email : " + email, ex);
                return true;
            }
        }

        public bool VerifyEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            if (m_Engine == null)
            {
                return Regex.IsMatch(email, EmailRegex2, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(10));
            }
            try
            {
                var verificationEmail = new Verification(email);
                m_Engine.Run(verificationEmail, m_VerificationLevel);
                return verificationEmail.State.Result.LastStatus == VerificationStatus.Success;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On check email email : " + email, ex);
                return true;
            }
        }

        public void Dispose()
        {
            m_Engine.Dispose();
        }
    }
}