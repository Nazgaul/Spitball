using System.IO;

namespace Zbang.Zbox.Domain
{
    public class File : Item
    {
        protected File()
        {
           
        }
        public File(string itemName, User uploaderUser, long sized, string blobName,
            Box box)
           : base(itemName, uploaderUser, sized, box, blobName)
        { }

        public virtual int NumberOfDownloads { get; private set; }

        public virtual string Content { get; set; }

        public virtual string Md5 { get; set; }

        public virtual bool PreviewFailed { get; set; }

       

       

        public void IncreaseNumberOfDownloads()
        {
            NumberOfDownloads++;
        }

        public override void DeleteAssociation()
        {
            base.DeleteAssociation();
            Content = null;
        }

        public override string ChangeName(string newName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newName);
            var ext=Path.GetExtension(Name);
            if (fileNameWithoutExtension == Path.GetFileNameWithoutExtension(Name))
            {
                return Name;
            }


            Name = fileNameWithoutExtension + ext;
            ShouldMakeDirty = () => true;
            GenerateUrl();
            return Name;
        }
    }
}