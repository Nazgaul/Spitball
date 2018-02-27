namespace Cloudents.Core.Entities.Db
{
    public class University 
    {
        protected University()
        {
        }

        public virtual long Id { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}
