namespace Zbang.Zbox.ViewModel.Dto.Dashboard
{
    public class WallDto 
    {
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public long UserId { get; set; }
        public long BoxId { get; set; }
        public string BoxName { get; set; }
        public string Action { get; set; }


        public string UniName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string Url { get; set; }



    }
}
