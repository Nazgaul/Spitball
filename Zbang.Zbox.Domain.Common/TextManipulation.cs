using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Zbang.Zbox.Domain.Common
{
    public static class TextManipulation
    {
        //public static string EncodeComment(string comment)
        //{
        //    if (string.IsNullOrEmpty(comment))
        //        return null;
        //    //remove those because the + sign is gone if we url decode it from 
        //    //var rawUserComment = HttpUtility.UrlDecode(comment);//We Get the comment from the js as escape chars. this is legacy
        //    var encodeUserComment = HttpUtility.HtmlEncode(comment);
        //    if (encodeUserComment == null) throw new ArgumentNullException(nameof(comment));
        //    encodeUserComment = DecodeUrls(encodeUserComment);
        //    return encodeUserComment;

        //}

        //public static string EncodeCommentWithoutUrl(string comment)
        //{
        //    if (string.IsNullOrEmpty(comment))
        //        return null;
        //    //remove those because the + sign is gone if we url decode it from 
        //    //var rawUserComment = HttpUtility.UrlDecode(comment);//We Get the comment from the js as escape chars. this is legacy
        //    return HttpUtility.HtmlEncode(comment);
        //}
        public static string EncodeText(string text)
        {
            return EncodeText(text, null);
        }

        public static string EncodeText(string text, params string[] allowElements)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            var sb = new StringBuilder(
                            HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(text)));
            //sb.Replace("&#34;", "\"");
            if (allowElements == null) return sb.ToString();
            foreach (var allowElement in allowElements)
            {
                sb.Replace($"&lt;{allowElement}&gt;", $"<{allowElement}>");
                sb.Replace($"&lt;/{allowElement}&gt;", $"</{allowElement}>");

                var str = sb.ToString();
                var index = str.IndexOf($"&lt;{allowElement}", StringComparison.Ordinal);
                while (index > -1)
                {
                    const string gt = "&gt;";
                    var indexEnd = str.IndexOf(gt, index, StringComparison.Ordinal);
                    var oldValue = str.Substring(index, indexEnd + gt.Length - index);
                    var newValue = oldValue;

                    newValue = newValue.Replace("&lt;", "<");
                    newValue = newValue.Replace("&gt;", ">");
                    newValue = newValue.Replace("&quot;", "\"");

                    sb.Replace(oldValue, newValue);
                    str = sb.ToString();
                    index = str.IndexOf($"&lt;{allowElement}", StringComparison.Ordinal);
                }
            }
            return Regex.Replace(sb.ToString(), "<(\\w+)\\b(?:\\s+[\\w\\-.:]+(?:\\s*=\\s*(?:\"[^\"]*\"|\"[^\"]*\"|[\\w\\-.:]+))?)*\\s*\\/?>\\s*<\\/\\1\\s*>", string.Empty);
        }


        //public static string DecodeUrls(string commentText)
        //{
        //    var sb = new StringBuilder(commentText);
        //    var match = UrlDetector.Match(commentText);
        //    while (match.Success)
        //    {
        //        var url = match.Value;
        //        if (Validation.IsUrlWithoutScheme(match.Value))
        //        {
        //            url = $"http://{url}";
        //        }
        //        sb.Replace(match.Value, $"<a target=\"_Blank\" href=\"{url}\">{match.Value}</a>");
        //        match = match.NextMatch();
        //    }
        //    return sb.ToString();
        //}


    }

}
