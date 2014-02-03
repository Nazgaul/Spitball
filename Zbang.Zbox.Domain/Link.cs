using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class Link
    {
        //Ctor
        internal protected Link()
        {
            DateTime now = DateTime.UtcNow;
            CreationTimeUtc = now;
            CreationTimeEpochMillis = Convert.ToInt64((now - ModelConstants.StartOfEpoch).TotalMilliseconds);
        }

        public Link(Box box, string url, Guid uploaderId)
            : this()
        {
            this.Box = box;
            this.Url = url;
            this.UploaderId = uploaderId;
        }

        public virtual int Id { get; set; }
        public virtual Box Box { get; set; }
        
        public virtual string Url { get; set; }
        public virtual DateTime CreationTimeUtc { get; set; }
        public virtual long CreationTimeEpochMillis { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual Guid UploaderId { get; set; }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = prime * Box.GetHashCode();
            result = prime * result + Url.GetHashCode();
            return result;
        }

        public override bool Equals(object obj)
        {
            Link otherLink = obj as Link;
            if (otherLink == null)
                return false;

            Box thisBox = this.Box;
            Box otherBox = otherLink.Box;          

            if (!thisBox.Equals(otherBox))
                return false;

            return this.Url.Equals(otherLink.Url);
        }
    }
}
