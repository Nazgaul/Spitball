using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Database.Maps
{
    public class DocumentMap : SpitballClassMap<Document>
    {
        public DocumentMap()
        {
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10",
                $"{nameof(HiLoGenerator.TableName)}='{nameof(Document)}'");

            Map(x => x.Name).Length(150).Not.Nullable();
            Map(x => x.BlobName).Not.Nullable();
            References(x => x.University).Column("UniversityId").ForeignKey("Document_University");
            HasManyToMany(x => x.Courses)
                .ParentKeyColumn("DocumentId")
                .ChildKeyColumn("CourseId")
                .ForeignKeyConstraintNames("Document_Courses", "Courses_Documents")
                .Table("DocumentsCourses").AsSet();
            Map(x => x.Type).Not.Nullable();

            HasManyToMany(x => x.Tags)
                .ParentKeyColumn("DocumentId")
                .ChildKeyColumn("TagId")
                .ForeignKeyConstraintNames("Document_Tags", "Tags_Documents")
                .Table("DocumentsTags").AsSet();

            
            Component(x => x.TimeStamp);
            References(x => x.User).Column("UserId").Not.Nullable().ForeignKey("Document_User");
            Map(x => x.Views).Not.Nullable();
            //Map(x => x.BlobName);
            //Map(x => x.Content).Length(500);
            //Map(x => x.Discriminator).Not.Nullable();
            //Map(x => x.IsDeleted);//.Index("iBoxIsDeleted");
            //References(e => e.Course).Column("BoxId").Not.Nullable();//.Index("iBoxIsDeleted");
            //Table("Item");
            //Schema("Zbox");

            SchemaAction.Update();
        }
    }
}