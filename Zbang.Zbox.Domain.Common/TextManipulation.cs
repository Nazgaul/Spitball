﻿using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Zbang.Zbox.Domain.Common
{
    public static class TextManipulation
    {
        public static string EncodeComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                return string.Empty;
            //remove those because the + sign is gone if we url decode it from 
            //var rawUserComment = HttpUtility.UrlDecode(comment);//We Get the comment from the js as escape chars. this is legacy
            var encodeUserComment = HttpUtility.HtmlEncode(comment);
            if (encodeUserComment == null) throw new ArgumentNullException("comment");
            encodeUserComment = DecodeUrls(encodeUserComment);
            return encodeUserComment;

        }

        public static string EncodeText(string text)
        {
            return EncodeText(text, null);
        }

        public static string EncodeText(string text, params string[] allowElements)
        {
            if (string.IsNullOrEmpty(text)) {
                return text;
            }
            var sb = new StringBuilder(
                            HttpUtility.HtmlEncode(text));
            if (allowElements != null)
            {
                foreach (var allowElement in allowElements)
                {
                    sb.Replace(string.Format("&lt;{0}&gt;", allowElement), string.Format("<{0}>", allowElement));
                    sb.Replace(string.Format("&lt;/{0}&gt;", allowElement), string.Format("</{0}>", allowElement));

                    var str = sb.ToString();
                    var index = str.IndexOf(string.Format("&lt;{0}", allowElement), StringComparison.Ordinal);
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
                        index = str.IndexOf(string.Format("&lt;{0}", allowElement), StringComparison.Ordinal);
                    }





                    //var match = Regex.Match(sb.ToString(), string.Format("&lt;{0}(.*)&gt;", allowElement));
                    //while (match.Success)
                    //{
                    //    // var matches = Regex.Matches(sb.ToString(), string.Format("&lt;{0}(.*)&gt;", allowElement));
                    //    const string replace = "&lt;";
                    //    var patternToReplace = string.Format("{1}{0}", allowElement, replace);
                    //    sb.Replace(patternToReplace, string.Format("<{0}", allowElement),
                    //        match.Index, patternToReplace.Length);
                    //    var indexOfGt = sb.ToString().IndexOf("&gt;", match.Index, StringComparison.InvariantCulture);
                    //    sb.Replace("&gt;", ">", indexOfGt, 4);
                    //    match = Regex.Match(sb.ToString(), string.Format("&lt;{0}(.*)&gt;", allowElement));
                    //}

                }
            }
            return sb.ToString();
        }

        public static readonly Regex UrlDetector = new Regex(@"(?i)\b((?:https?://|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))", RegexOptions.IgnoreCase);


        public static string DecodeUrls(string commentText)
        {
            var sb = new StringBuilder(commentText);
            var match = UrlDetector.Match(commentText);
            while (match.Success)
            {
                var url = match.Value;
                if (Validation.IsUrlWithoutSceme(match.Value))
                {
                    url = string.Format("http://{0}", url);
                }
                sb.Replace(match.Value, string.Format("<a target=\"_Blank\" href=\"{0}\">{1}</a>", url, match.Value));
                //commentText = commentText.Replace(match.Value, string.Format("<a href=\"{0}\">{0}</a>", match.Value));

                match = match.NextMatch();
            }
            return sb.ToString();
        }

        //public static string CombineUserServerComments(string userComment, string serverComment)
        //{
        //    if (string.IsNullOrEmpty(userComment))
        //    {
        //        userComment = string.Empty;
        //    }

        //    string encodeUserComment = EncodeComment(userComment);

        //    var sb = new StringBuilder(encodeUserComment.Trim());

        //    if (sb.Length > 0)
        //    {
        //        sb.Append("\n");
        //    }
        //    sb.AppendFormat(serverComment);

        //    return sb.ToString();
        //}

        //public static bool CombineFileServerComments(string sessionId, int batchSize, out string fileServerComments)
        //{
        //    var fileCommentList = HttpContext.Current.Session[sessionId] as List<string>;
        //    if (fileCommentList == null)
        //    {
        //        throw new NullReferenceException("file comment is null");
        //    }
        //    fileServerComments = string.Join(", ", fileCommentList);
        //    if (fileCommentList.Count == batchSize)
        //        HttpContext.Current.Session.Remove(sessionId);
        //    return fileCommentList.Count == batchSize;
        //}

        //public static void AddFileServerComments(long itemid, string fileName, string sessionId)
        //{

        //    string comment = string.Format("<a data-href=\"{0}\" href=\"#\">{1}</a>", itemid, fileName);
        //    var fileCommentList = HttpContext.Current.Session[sessionId] as List<string>;
        //    if (fileCommentList == null) // new batch of upload
        //    {
        //        fileCommentList = new List<string> { comment };
        //        HttpContext.Current.Session[sessionId] = fileCommentList;
        //    }
        //    else
        //    {
        //        fileCommentList.Add(comment);
        //    }
        //}


    }

}
