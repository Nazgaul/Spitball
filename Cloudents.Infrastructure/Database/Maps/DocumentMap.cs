using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
{
    public class DocumentMap : ClassMap<Document>
    {
        public DocumentMap()
        {
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10",
                $"{nameof(HiLoGenerator.TableName)}='{nameof(Document)}'");

            Map(x => x.Name).Length(150).Not.Nullable();
           // Map(x => x.BlobName).Not.Nullable();
            References(x => x.University).Column("UniversityId").ForeignKey("Document_University");
           
            Map(x => x.Type).Not.Nullable();

            HasManyToMany(x => x.Tags)
                .ParentKeyColumn("DocumentId")
                .ChildKeyColumn("TagId")
                .ForeignKeyConstraintNames("Document_Tags", "Tags_Documents")
                .Table("DocumentsTags").AsSet();

            
            Component(x => x.TimeStamp);
            References(x => x.Course).Column("CourseName").Not.Nullable().ForeignKey("Document_course");
            References(x => x.User).Column("UserId").Not.Nullable().ForeignKey("Document_User");
            Map(x => x.Views).Not.Nullable();
            Map(x => x.Downloads).Not.Nullable();
            Map(x => x.Professor).Nullable();
            Map(x => x.PageCount).Nullable();
            Map(x => x.Language).Nullable();
            Map(x => x.Purchased).Not.Nullable();
            Map(x => x.OldId).Nullable();
            Map(x => x.State).Nullable();
            
            SchemaAction.Update();
        }
    }
}