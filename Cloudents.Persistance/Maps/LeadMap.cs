using Cloudents.Core.Entities;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
//using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class LeadMap : ClassMapping<Lead>
    {
        public LeadMap()
        {
            Id(x => x.Id, c=> c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            ManyToOne(x => x.Course, c => {
                c.NotNullable(true);
                c.Column("CourseId");
            });
            //References(x => x.Course).Not.Nullable();
            ManyToOne(x => x.User, c => {
                c.NotNullable(false);
                c.Column("UserId");
            });
            //References(x => x.User).Nullable();
            ManyToOne(x => x.Tutor, c => {
                c.NotNullable(false);
                c.Column("TutorId");
            });
            //References(x => x.Tutor).Nullable();
            Property(x => x.Referer, c => c.Length(400));
            //Map(x => x.Referer).Length(400);
            Property(x => x.Text, c => c.Length(1000));
            //Map(x => x.Text).Length(1000);
            Property(x => x.CreationTime);
            //Map(x => x.CreationTime);
            Property(x => x.UtmSource);
            //Map(x => x.UtmSource);

            Component(x => x.Status);

            //Map(x => x.Status);

        }
    }
}