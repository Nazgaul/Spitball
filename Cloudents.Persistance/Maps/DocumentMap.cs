using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public sealed class DocumentMap : ClassMap<Document>
    {
        public DocumentMap()
        {
            DynamicUpdate();
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10",
                $"{nameof(HiLoGenerator.TableName)}='{nameof(Document)}'");

            Map(x => x.Name).Length(150).Not.Nullable();
            
            Component(x => x.TimeStamp);
            
            //Cannot put not nullable because no inverse on course.
            References(x => x.Course).Column("CourseId").ForeignKey("Document_course2");


            References(x => x.User).Column("UserId").Not.Nullable().ForeignKey("Document_User");
            Map(x => x.Views).Not.Nullable();
            Map(x => x.Downloads).Not.Nullable();
            Map(x => x.PageCount).Nullable();
            Map(x => x.MetaContent).Nullable();
            Map(x => x.Md5).Nullable();
            //HasMany(x => x.Transactions)
            //    .KeyColumn("DocumentId")
            //    //.Cascade.()
            //    .Access.CamelCaseField(Prefix.Underscore).ExtraLazyLoad()
            //    .Inverse();
           

            HasMany(x => x.DocumentDownloads)
             .Cascade.AllDeleteOrphan()
             .KeyColumn("DocumentId").Inverse().AsSet();

            Map(x => x.DocumentType).Column("DocumentType");
            Map(x => x.Duration);
            Component(x => x.Status);
            Map(x => x.Position).ReadOnly();
        }
    }


}