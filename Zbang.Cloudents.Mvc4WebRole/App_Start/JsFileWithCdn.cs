using System.IO;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public class JsFileWithCdn
    {
        public JsFileWithCdn(string localFile)
        {
            LocalFile = localFile;
        }
        // ReSharper disable once UnusedMember.Local
        public JsFileWithCdn(string localFile, string cdnFile)
        {
            LocalFile = localFile;
            CdnFile = cdnFile;
        }

        public string LocalFile { get; }
        public string CdnFile { get; }
    }

    public class CssWithRtl
    {
        public CssWithRtl(string leftCssFile)
        {
            LeftCssFile = leftCssFile;
            RightCssFile = $"{LeftCssFile.Replace(Path.GetExtension(LeftCssFile), string.Empty)}.rtl.css";
        }

        public CssWithRtl(string leftCssFile,string rightCssFile)
        {
            LeftCssFile = leftCssFile;
            RightCssFile = rightCssFile;
        }

        public string RightCssFile { get; }
        public string LeftCssFile { get;  }
    }
}