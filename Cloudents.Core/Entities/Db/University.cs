namespace Cloudents.Core.Entities.Db
{
    public class University 
    {
        protected University()
        {
        }

        public virtual long Id { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual string Name { get; set; }
        public virtual string Extra { get; set; }
        public virtual string ExtraSearch { get; set; }
    }
}
