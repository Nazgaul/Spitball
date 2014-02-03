using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yahoo.Yui.Compressor;


namespace Zbang.Zbox.Mvc3WebRole.App_Start.AzureCdn
{
    public static class CompressAndCombineCssJsFiles
    {
        static CssCompressor c = new CssCompressor();
        static JavaScriptCompressor j = new JavaScriptCompressor();
        public static string CombineCssFiles(IEnumerable<string> files)
        {
            return CombineFiles(files, css =>
                {
                   
                    
                    //css = css.Replace("url('/Content/", "url('/cdn/Content/");
                    //css = css.Replace("url(\"/Content/", "url(\"/cdn/Content/");
                    var compressedCss = c.Compress(css);//, int.MaxValue, CssCompressionType.StockYuiCompressor);
                    return compressedCss;
                });
            //StringBuilder sb = new StringBuilder();
            //foreach (var cssfile in files)
            //{
            //    var css = File.ReadAllText(cssfile);
            //    css = css.Replace("url('/Content/", "url('/cdn/Content/");
            //    css = css.Replace("url(\"/Content/", "url(\"/cdn/Content/");
            //    var compressedCss = CssCompressor.Compress(css, int.MaxValue, CssCompressionType.StockYuiCompressor);
            //    sb.Append(compressedCss);
            //}
            //return sb.ToString();

        }

        public static string CombineJsFiles(IEnumerable<string> files)
        {
            return CombineFiles(files, js =>
               {
                   
                   //js = js.Replace("/Content/", "/cdn/Content/");
                   var compressedJs = j.Compress(js);
                   return compressedJs;
               });
            //StringBuilder sb = new StringBuilder();
            //foreach (var jsfile in files)
            //{
            //    var js = File.ReadAllText(jsfile);
            //    var compressedJs = JavaScriptCompressor.Compress(js);
            //    sb.Append(compressedJs);
            //}
            //return sb.ToString();

        }

        public static string CombineFiles(IEnumerable<string> files, Func<string, string> doCompression)
        {
            var sb = new StringBuilder();
            foreach (var file in files)
            {
                var fileContent = File.ReadAllText(file);
                sb.Append(doCompression.Invoke(fileContent));
                //do some function
                //sb.Append(compressedJs);
            }
            return sb.ToString();
        }

    }
}