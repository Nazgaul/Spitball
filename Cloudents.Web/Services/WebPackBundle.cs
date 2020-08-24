namespace Cloudents.Web.Services
{
    public class WebPackBundle
    {
        public string Css { get; set; }

        public string RtlCss { get; set; }
        public string Js { get; set; }


        public string GetCss()
        {
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                return RtlCss;
            }

            return Css;
        }
    }
}