namespace Zbang.Zbox.Domain
{
    public class Link : Item
    {
        protected Link()
        {
            ShouldMakeDirty = () => false;
        }

        public Link(string itemName, User uploaderUser, long sized, Box box,
            string linkTitle)
            : base(linkTitle, uploaderUser, sized, box, itemName)
        { }





        public override string ChangeName(string newName)
        {
            Name = newName;
            GenerateUrl();
            return Name;
        }
    }
}