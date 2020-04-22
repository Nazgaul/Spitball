namespace Cloudents.Persistence
{
    internal class HiLoGenerator
    {
        public virtual int Id { get; set; }
        public virtual string TableName { get; set; }
        public virtual long NextHi { get; set; }
    }
}
