using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cobisi.EmailVerify;
using RestSharp;
using RestSharp.Authenticators;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public sealed class EmailVerification : IEmailVerification, IDisposable
    {
        private readonly VerificationEngine m_Engine;
        private readonly VerificationLevel m_VerificationLevel = VerificationLevel.Dns;
        private const string EmailRegex2 = @"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-z0-9]{1}[a-z0-9\-]{0,62}[a-z0-9]{1})|[a-z])\.)+[a-z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$";

        private class MailGunValidationResponse
        {
            public bool Is_Valid { get; set; }
        }
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
                if (verificationEmail.State.Result.LastStatus == VerificationStatus.UnhandledException)
                {
                    return await MailGunValidateEmailAsync(email);
                   
                }
                return verificationEmail.State.Result.LastStatus == VerificationStatus.Success;
            }
            catch (Exception ex)
            {

                TraceLog.WriteError("On check email email : " + email, ex);
                return true;
            }
        }

        private static RestClient BuildRestClient(string email, out RestRequest request)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri("https://api.mailgun.net/v3"),
                Authenticator = new HttpBasicAuthenticator("api",
                    "pubkey-871e78a663947b3b87c523a7b81c4b78")
            };
            request = new RestRequest
            {
                Resource = "/address/validate",
            };

            request.AddParameter("address", email);
            return client;
        }

        private static bool MailGunValidateEmail(string email)
        {
            RestRequest request;
            var client = BuildRestClient(email, out request);
            var data = client.Execute<MailGunValidationResponse>(request);
            return data.Data.Is_Valid;
        }

        private static async Task<bool> MailGunValidateEmailAsync(string email)
        {
            RestRequest request;
            var client = BuildRestClient(email, out request);
            var data = await client.ExecuteTaskAsync<MailGunValidationResponse>(request);
            return data.Data.Is_Valid;
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
                if (verificationEmail.State.Result.LastStatus == VerificationStatus.UnhandledException)
                {
                    return MailGunValidateEmail(email);
                }
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