using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class StripePaymentMap : ClassMap<StripePayment>
    {
        public StripePaymentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(z => z!.PaymentKey);
            Map(x => x.Created);
        }
    }
}