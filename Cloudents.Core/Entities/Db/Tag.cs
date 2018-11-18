using System;

namespace Cloudents.Core.Entities.Db
{
    
    public class Tag
    {
        public const int MinLength = 3;
        public const int MaxLength = 150;
        protected Tag()
        {

        }

        public Tag(string name)
        {
            Name = name.Trim();
            if (Name.Length > MaxLength || Name.Length < MinLength)
            {
                throw new ArgumentException();
            }

            if (Name.Contains(","))
            {
                throw new ArgumentException();
            }
        }


        public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }
    }
}