﻿using System;
using System.Collections.Generic;
using SendGrid;

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
           // message.AddSubVal("{USERNAME}", new List<string> { forgotParams2.Name });
        }

        //public void OldForgotPassword(SendGridMail.ISendGrid message, MailParameters parameters)
        //{
        //    var forgotParams = parameters as ForgotPasswordMailParams;
        //    Zbang.Zbox.Infrastructure.Exceptions.Throw.OnNull(forgotParams, "forgotParams");

            
        //    message.SetCategory(Category);
        //    message.Html = LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ForgotPwdOld");
        //    //message.Text = textBody;
        //    message.Subject = Subject;
        //    message.AddSubVal("{NEW-PWD}", new List<string> { forgotParams.NewPassword });
        //}
    }
}
