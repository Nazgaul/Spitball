using System;
using System.Linq;
using System.Text;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser.SyntaxTree;

namespace Zbang.Cloudents.Mobile.Helpers.Mimify
{
    public sealed class MinifyHtmlCodeGenerator : CSharpRazorCodeGenerator
    {
        public MinifyHtmlCodeGenerator(string className, string rootNamespaceName, string sourceFileName, RazorEngineHost host)
            : base(className, rootNamespaceName, sourceFileName, host)
        {
        }
        public override void VisitSpan(Span span)
        {
            //System.Web.Razor.Parser.SyntaxTree.
            // We only minify the static text
            if (span.Kind != SpanKind.Markup)
            {
                base.VisitSpan(span);
                return;
            }
            //var markupSpan = span as MarkupSpan;
            //if (markupSpan == null)
            //{
            //    base.VisitSpan(span);
            //    return;
            //}
            var content = span.Content;
            content = Minify(content);
            span.GetType().GetProperty("Content").SetValue(span, content, null);
            //span.Content = content;
            base.VisitSpan(span);
        }
        private string Minify(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }
            var builder = new StringBuilder(content.Length);
            // Minify the comments
            var icommentstart = content.IndexOf("<!--", StringComparison.Ordinal);
            while (icommentstart >= 0)
            {
                var icommentend = content.IndexOf("-->", icommentstart + 3, StringComparison.Ordinal);
                if (icommentend < 0)
                {
                    break;
                }
                if (CommentsMarkers.Select(m => content.IndexOf(m, icommentstart, StringComparison.Ordinal)).Any(i => i > 0 && i < icommentend))
                {
                    // There is a comment but it contains javascript or IE conditionals
                    // => we keep it
                    break;
                }
                builder.Append(content, 0, icommentstart);
                builder.Append(content, icommentend + 3, content.Length - icommentend - 3);
                content = builder.ToString();
                builder.Clear();
                icommentstart = content.IndexOf("<!--", icommentstart, StringComparison.Ordinal);
            }
            // Minify white space while keeping the HTML compatible with the given one
            var lines = content.Split(WhiteSpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var trimmedLine = line.Trim();
                if (trimmedLine.Length == 0)
                {
                    continue;
                }
                if (char.IsWhiteSpace(line[0]) && (trimmedLine[0] != '<'))
                {
                    builder.Append(' ');
                }
                builder.Append(trimmedLine);
                if (char.IsWhiteSpace(line[line.Length - 1]) && (trimmedLine[trimmedLine.Length - 1] != '>'))
                {
                    builder.Append(' ');
                }
                if ((i < lines.Length - 1) || (WhiteSpaceSeparators.Any(s => s == content[content.Length - 1])))
                {
                    builder.Append('\n');
                }
            }
            return builder.ToString();
        }

        private static readonly char[] WhiteSpaceSeparators = { '\n', '\r' };
        private static readonly string[] CommentsMarkers = { "{", "}", "function", "var", "[if", "ko" };
    }

}