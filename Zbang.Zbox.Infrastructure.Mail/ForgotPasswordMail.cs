using System;
using System.Collections.Generic;
using SendGrid;
using System.Net.Http;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class ForgotPasswordMail : IMailBuilder
    {
        const string Category = "Password Recovery";
        const string Subject = "Password Recovery";


        public void GenerateMail(ISendGrid message, MailParameters parameters)
        {
            //var forgotParams = parameters as ForgotPasswordMailParams;
            //if (forgotParams != null)
            //{
            //    OldForgotPassword(message, forgotParams);
            //    return;
            //}
            var forgotParams2 = parameters as ForgotPasswordMailParams2;
            if (forgotParams2 == null)
            {
                throw new NullReferenceException("forgotParams2");
            }

            //message.DisableClickTracking();
            
            message.SetCategory(Category);
            message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ResetPwd");
           
            message.Subject = Subject;
            message.AddSubstitution("{NEW-PWD}", new List<string> { forgotParams2.Code });
            message.AddSubstitution("{CHANGE-URL}", new List<string> { forgotParams2.Link });
        }


        //private void DeleteUnsubscribe(string email)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {

        //       var content = new StringContent()
        //        client.PostAsync("https://api.sendgrid.com/api/user.unsubscribes.json",new HttpContent)
        //    }
        //}
       
    }
}
