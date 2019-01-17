using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistance.Maps
{
    public class LanguageMap : ClassMap<Language>
    {
        public LanguageMap()
        {
            Id(x => x.Id).Column("Name").Length(100).GeneratedBy.Assigned();
            SchemaAction.Validate();
        }
    }


    public class SystemEventsMap : ClassMap<SystemEvent>
    {
        public SystemEventsMap()
        {
            Id(x => x.Id).Column("Name").Length(100).GeneratedBy.Assigned();
            SchemaAction.Validate();
        }
    }


    public class EmailMap : ClassMap<Email>
    {
        public EmailMap()
        {
            ReadOnly();
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Subject).Not.Nullable();
            Map(x => x.SocialShare).Not.Nullable();
            References(x => x.Language, "Language").ForeignKey("Email_Language");
            References(x => x.Event, "Event").ForeignKey("Email_Event");


            Component(x => x.EmailBlock1, y =>
            {
                y.Map(x => x.Title).Column("Title1");
                y.Map(x => x.Subtitle).Column("Subtitle1");
                y.Map(x => x.Body).Column("Body1").Length(1000);
                y.Map(x => x.Cta).Column("cta1");
            });

            Component(x => x.EmailBlock2, y =>
            {
                y.Map(x => x.Title).Column("Title2");
                y.Map(x => x.Subtitle).Column("Subtitle2");
                y.Map(x => x.Body).Column("Body2").Length(1000);
                y.Map(x => x.Cta).Column("cta2");
            });
           

            SchemaAction.Validate();

        }
    }
}