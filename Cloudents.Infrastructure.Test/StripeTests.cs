//using System.Reflection;
//using Cloudents.Infrastructure.Payments;
//using FluentAssertions;
//using Stripe;
//using Xunit;

//namespace Cloudents.Infrastructure.Test
//{
//    public class StripeTests
//    {
//        [Fact]
//        public void CheckAssignPaymentToProduct_Ok()
//        {
//            var options = new PriceCreateOptions
//            {
//                Currency = "usd",
//                Recurring = new PriceRecurringOptions
//                {
//                    Interval = "month",
//                },
//                UnitAmount = (long)(5 * 100),
//            };

//            //options.AssignProduct("XXX");


//            var result = typeof(PriceCreateOptions).GetProperty("Product",BindingFlags.NonPublic |BindingFlags.Instance)!.GetValue(options)!.ToString();
//            result.Should().BeEquivalentTo("XXX");
//        }
//    }
//}