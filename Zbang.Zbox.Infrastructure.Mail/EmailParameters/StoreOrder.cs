
namespace Zbang.Zbox.Infrastructure.Mail.EmailParameters
{
   public class StoreOrder : MailParameters
    {
        public StoreOrder(string userName, string productName, long orderNumber) :
            base(new System.Globalization.CultureInfo("he-IL"))
        {
            UserName = userName;
            ProductName = productName;
            OrderNumber = orderNumber;
        }

        public string UserName { get; private set; }
        public string ProductName { get; private set; }
        public long OrderNumber { get; private set; }

        public override string MailResover
        {
            get { return OrderResolver; }
        }
    }
}
