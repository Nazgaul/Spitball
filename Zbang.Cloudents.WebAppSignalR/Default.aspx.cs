using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Zbang.Cloudents.WebAppSignalR
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            form1.Attributes.Add("data-parent", Zbox.Infrastructure.Extensions.ConfigFetcher.Fetch("WebSiteUrl"));
        }
    }
}