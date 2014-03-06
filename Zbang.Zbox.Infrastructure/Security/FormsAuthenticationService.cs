using System;
using System.Globalization;
using System.Web;
using System.Web.Security;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(long userId, bool rememberMe, UserDetail userDetail)
        {
            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(userId.ToString(CultureInfo.InvariantCulture),
                rememberMe);

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            var newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name,
                ticket.IssueDate, ticket.Expiration, rememberMe, UserDetail.Serialize(userDetail));
            authCookie.Value = FormsAuthentication.Encrypt(newTicket);
            if (ticket.IsPersistent)
            {
                authCookie.Expires = ticket.Expiration;
            }
            HttpContext.Current.Response.Cookies.Set(authCookie);
        }


        public UserDetail GetUserData()
        {
            const string itemInex = "UserData";
            var userData = HttpContext.Current.Items[itemInex] as UserDetail;
            if (userData == null)
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    return null;
                }
                var ident = HttpContext.Current.User.Identity as FormsIdentity;
                if (ident == null)
                {
                    return null;
                }
                var v = ident.Ticket.UserData;
                userData = UserDetail.Deserialize(v);

                HttpContext.Current.Items[itemInex] = userData;
            }
            return userData;
        }

       
        public void ChangeLanguage(string newLanguage)
        {
            Throw.OnNull(newLanguage, "newLanguage");
            var ident = (FormsIdentity)HttpContext.Current.User.Identity;
            var ticket = ident.Ticket;
            var userDetail = UserDetail.Deserialize(ticket.UserData);
            userDetail.Language = newLanguage;
            SignIn(Convert.ToInt64(ident.Name), ident.Ticket.IsPersistent, userDetail);
        }
        public void ChangeUniversity(long? universityId, long? universityWrapperId)
        {
            var ident = (FormsIdentity)HttpContext.Current.User.Identity;
            var ticket = ident.Ticket;
            var userDetail = UserDetail.Deserialize(ticket.UserData);
            userDetail.UniversityId = universityId;
            userDetail.UniversityWrapperId = universityWrapperId;
            SignIn(Convert.ToInt64(ident.Name), ident.Ticket.IsPersistent, userDetail);
        }

       

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
