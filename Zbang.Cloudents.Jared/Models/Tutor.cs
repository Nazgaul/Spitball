namespace Zbang.Cloudents.Jared.Models
{
    public class Tutor
    {
        public string Subject { get; set; }
        //private IEnumerable<string> keyList;
        public string KeyWords { get; set; }
        public string Education { get; set; }
        public string Description { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Location { get; set; }
        public int? Views { get; set; }
        public int? Likes { get; set; }
        //public IEnumerable<string> getKeyList()
        //{
        //    if (keyList == null || !keyList.Any())
        //    {
        //        keyList = KeyWords.Split(',');
        //    }
        //    return keyList;
        //}
    }
}