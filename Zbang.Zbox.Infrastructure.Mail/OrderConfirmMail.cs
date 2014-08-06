using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail.EmailParameters;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class OrderConfirmMail : IMailBuilder
    {

        public void GenerateMail(SendGrid.ISendGrid message, MailParameters parameters)
        {
            var storeParams = parameters as StoreOrder;
            if (storeParams == null)
            {
                throw new NullReferenceException("partnersParams");
            }
            var sb = new StringBuilder(LoadMailTempate.LoadMailFromContent(parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Store.OrderConfirmation"));
            message.Subject = "Cloudents order confirmation";
            message.SetCategory("Store Order");
            sb.Replace("{USERNAME}", storeParams.UserName)
              .Replace("{PRODUCT_NAME}", storeParams.ProductName)
              .Replace("{ORDER_NO}", storeParams.OrderNumber.ToString(CultureInfo.InvariantCulture));

            message.Html = sb.ToString();
        }
    }
}
