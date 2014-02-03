using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zbang.Zbox.Domain
{
    public class File
    {
        //Ctor
        internal protected File()
        {
            DateTime now = DateTime.UtcNow;
            CreationTimeUtc = now;
            CreationTimeEpochMillis = Convert.ToInt64((now - ModelConstants.StartOfEpoch).TotalMilliseconds);
        }

        //Properties
        public virtual int FileId { get; set; }
        public virtual string BlobAddressUri { get; set; }
        public virtual string ThumbnailBlobAddressUri { get; set; }
        public virtual int BoxId { get; set; }
        public virtual string FileName { get; set; }
        public virtual DateTime CreationTimeUtc { get; set; }
        public virtual long CreationTimeEpochMillis { get; set; }
        public virtual string ContentType { get; set; }
        public virtual long Length { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual Guid UploaderId { get; set; }

        //Methods   
        public override bool Equals(object obj)
        {
            File other = obj as File;
            if (other == null)
                return false;

            return FileName.Equals(other.FileName);
        }

        public override int GetHashCode()
        {
            return FileName.GetHashCode();
        }
    }
}
