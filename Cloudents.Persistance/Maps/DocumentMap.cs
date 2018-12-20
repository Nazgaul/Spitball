using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistance.Maps
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
            Component(x => x.Item, t =>
            {
                QuestionMap.ItemComponentPartialMapping(t);
                t.HasMany(x => x.Votes).KeyColumns.Add("DocumentId")
                    .Inverse().Cascade.AllDeleteOrphan();
                t.References(x => x.FlaggedUser).Column("FlaggedUserId").ForeignKey("DocumentFlagged_User");
            });
            References(x => x.Course).Column("CourseName").Not.Nullable().ForeignKey("Document_course");
            References(x => x.User).Column("UserId").Not.Nullable().ForeignKey("Document_User");
            Map(x => x.Views).Not.Nullable();
            Map(x => x.Downloads).Not.Nullable();
            Map(x => x.Professor).Nullable();
            Map(x => x.PageCount).Nullable();
            Map(x => x.Purchased).Not.Nullable();
            Map(x => x.OldId).Nullable();
            Map(x => x.MetaContent).Nullable();
            
            SchemaAction.None();
            //DiscriminateSubClassesOnColumn("State");
        }
    }


    //public class DocumentDeletedMap : SubclassMap<DocumentDeleted>
    //{
    //    public DocumentDeletedMap()
    //    {

    //        DiscriminatorValue(ItemState.Deleted);
    //    }
    //}

    //public class DocumentPendingMap : SubclassMap<DocumentPending>
    //{
    //    public DocumentPendingMap()
    //    {

    //        DiscriminatorValue(ItemState.Pending);
    //    }
    //}

    //public class DocumentApprovedMap : SubclassMap<DocumentApproved>
    //{
    //    public DocumentApprovedMap()
    //    {
    //        DiscriminatorValue(ItemState.Ok);
    //    }
    //}
}