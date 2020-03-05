using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class PayPalMap : ClassMap<PayPal>
    {
        public PayPalMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(m => m.Token).Not.Nullable();
            Map(m => m.PaymentApproved).Not.Nullable();
        }
    }
}
