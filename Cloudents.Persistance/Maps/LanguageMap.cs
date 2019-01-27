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
            SchemaAction.Update();
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
            HasMany(x => x.EmailBlock);
            SchemaAction.Update();
        }
    }

    public class EmailBlockMap : ClassMap<EmailBlock>
    {
        public EmailBlockMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.Title);
            Map(x => x.SubTitle);
            Map(x => x.MinorTitle);
            Map(x => x.Body).Length(1000);
            Map(x => x.Cta);
            Map(x => x.Order).Column("OrderBlock");

            ReadOnly();
            SchemaAction.Update();
        }
    }
}