using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Quiz
    {
        protected Quiz()
        {
            
        }
        public Quiz(string name, long id, Box box, User owner)
            :this()
        {
            Id = id;
            Name = name;
            Owner = owner;
            Box = box;

            DateTimeUser = new UserTimeDetails(owner.Email);
        }
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }

        public virtual bool Publish { get; set; }

        public virtual User Owner { get; set; }
        public virtual Box Box { get; set; }

        public virtual string Content { get; set; }

        public virtual float Rate { get; set; }
        public virtual int NumberOfViews { get; set; }
        public virtual int NumberOfComments { get; set; }
        public virtual UserTimeDetails DateTimeUser { get; set; }
    }
}
