using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class PaymeMap : ClassMap<Payme>
    {
        public PaymeMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.PaymentApproved).Not.Nullable();
            Map(x => x.AdminDuration).Not.Nullable();
            Map(x => x.StudentPay).CustomSqlType("smallMoney").Not.Nullable();
            Map(x => x.SpitballPay).CustomSqlType("smallMoney").Not.Nullable();
        }
    }
}
