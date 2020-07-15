using System.Collections.Generic;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Query.Payment
{



    public class GenerateSaleResponse
    {
        public int StatusCode { get; set; }

        public string StatusErrorDetails { get; set; }
        public string SaleUrl { get; set; }
        public string PaymeSaleId { get; set; }
        public int PaymeSaleCode { get; set; }
        public int Price { get; set; }
        public string TransactionId { get; set; }
        public string Currency { get; set; }
        public string SalePaymentMethod { get; set; }
        public string TransactionCcAuthNumber { get; set; }
    }

    public class StripePaymentRequest
    {
        public StripePaymentRequest(string name, Money money, string email, string successCallback, string fallbackCallback)
        {
            Name = name;
            Money = money;
            Email = email;
            SuccessCallback = successCallback;
            FallbackCallback = fallbackCallback;
        }

        public string Name { get;  }

        public Money Money { get; }

        public Dictionary<string,string> Metadata { get; set; }

        public string Email { get;  }

        public string SuccessCallback { get;  }
        public string FallbackCallback { get;  }

    }

}