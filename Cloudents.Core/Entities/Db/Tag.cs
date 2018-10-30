using System;

namespace Cloudents.Core.Entities.Db
{
    public class Tag
    {
        protected Tag()
        {

        }

        public Tag(string name)
        {
            Name = name.Trim();
            if (Name.Length > 150 || Name.Length < 4)
            {
                throw new ArgumentException();
            }
        }


        public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }
    }
}