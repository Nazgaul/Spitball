using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Zbang.Cloudents.MobileApp.Models
{
    public class MySqlMembershipProvider : SqlMembershipProvider
    {
        public override void Initialize(string name, NameValueCollection config)
        {
            config["connectionString"] = ConfigurationManager.ConnectionStrings["Zbox"].ConnectionString;
            base.Initialize(name, config);
        }

        public override string GetUserNameByEmail(string email)
        {
            var x = ConfigurationManager.ConnectionStrings["Zbox"].ConnectionString;
            return base.GetUserNameByEmail(email);
        }
    }
}