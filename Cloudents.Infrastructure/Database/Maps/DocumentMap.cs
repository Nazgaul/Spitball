using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Database.Maps
{
    public class DocumentMap : SpitballClassMap<Document>
    {
        public DocumentMap()
        {
            //Id(x => x.Id).Column("ItemId").GeneratedBy.Native();
            //Map(x => x.Name).Length(4000);
            //Component(x => x.RowDetail);
            //Map(x => x.BlobName);
            //Map(x => x.Content).Length(500);
            //Map(x => x.Discriminator).Not.Nullable();
            //Map(x => x.IsDeleted);//.Index("iBoxIsDeleted");
            //References(e => e.Course).Column("BoxId").Not.Nullable();//.Index("iBoxIsDeleted");
            //Table("Item");
            //Schema("Zbox");

            //SchemaAction.None();
        }
    }
}