namespace Zbang.Zbox.Domain
{
    public class Link : Item
    {
        protected Link()
        {
            ShouldMakeDirty = () => false;
        }
        public Link(string itemName, User uploaderUser, long sized, Box box,
            string linkTitle, string thumbnailBlobName, string thumbnailUrl)
            : base(linkTitle, uploaderUser, sized, box, itemName, thumbnailBlobName, thumbnailUrl)
        { }





        public override string ChangeName(string newName)
        {
            Name = newName;
            GenerateUrl();
            return Name;
        }
    }
}