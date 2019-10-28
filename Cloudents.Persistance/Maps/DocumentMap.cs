using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public sealed class DocumentMap : ClassMap<Document>
    {
        public DocumentMap()
        {
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10",
                $"{nameof(HiLoGenerator.TableName)}='{nameof(Document)}'");

            Map(x => x.Name).Length(150).Not.Nullable();
            References(x => x.University).Not.Nullable().Column("UniversityId").ForeignKey("Document_University");
           
           

         

            
            Component(x => x.TimeStamp);
           
            References(x => x.Course).Column("CourseName").Not.Nullable().ForeignKey("Document_course");
            References(x => x.User).Column("UserId").Not.Nullable().ForeignKey("Document_User");
            Map(x => x.Views).Not.Nullable();
            Map(x => x.Downloads).Not.Nullable();
            Map(x => x.PageCount).Nullable();
            Map(x => x.Description).Nullable();
            Map(x => x.MetaContent).Nullable();
            Map(x => x.Price).Not.Nullable().CustomSqlType("smallmoney"); 
            //DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            HasMany(x => x.Transactions)
                .KeyColumn("DocumentId")
                //.Cascade.()
                .Access.CamelCaseField(Prefix.Underscore)
                .LazyLoad()
                .Inverse();
            Map(x => x.OldId).Nullable();
            HasMany(x => x.Votes)
                .Access.CamelCaseField(Prefix.Underscore)
                .KeyColumns.Add("DocumentId")
                .Inverse().Cascade.AllDeleteOrphan();
            Map(m => m.VoteCount);


            Map(x => x.DocumentType).Column("DocumentType");
            Map(x => x.Duration);//.CustomType<TimeAsTimeSpanType>();


            Component(x => x.Status);
        }
    }

    
}