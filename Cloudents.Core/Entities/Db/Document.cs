namespace Cloudents.Core.Entities.Db
{
    public class Document
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        //public virtual RowDetail RowDetail { get; protected set; }
        public virtual string BlobName { get; protected set; }
        


        public virtual string Discriminator { get; protected set; }

        public virtual bool IsDeleted { get; protected set; }

        public virtual string Content { get; protected set; }

        public virtual Course Course { get; set; }
    }
}