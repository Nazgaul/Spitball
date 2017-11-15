using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class YoutubeProcessor : LinkProcessor
    {
        public YoutubeProcessor(IBlobProvider blobProvider, IBlobProvider2<IPreviewContainer> blobProviderPreview, ILogger logger)
            : base(blobProvider, blobProviderPreview, logger)
        { }

        private static readonly Regex YoutubeRegex = new Regex(
           @"# Match non-linked youtube URL in the wild. (Rev:20111012)
            https?://         # Required scheme. Either http or https.
            (?:[0-9A-Z-]+\.)? # Optional subdomain.
            (?:               # Group host alternatives.
              youtu\.be/      # Either youtu.be,
            | youtube\.com    # or youtube.com followed by
              \S*             # Allow anything up to VIDEO_ID,
              [^\w\-\s]       # but char before ID is non-ID char.
            )                 # End host alternatives.
            ([\w\-]{11})      # $1: VIDEO_ID is exactly 11 chars.
            (?=[^\w\-]|$)     # Assert next char is non-ID or EOS.
            (?!               # Assert URL is not pre-linked.
              [?=&+%\w]*      # Allow URL (query) remainder.
              (?:             # Group pre-linked alternatives.
                [\'""][^<>]*> # Either inside a start tag,
              | </a>          # or inside <a> element text contents.
              )               # End recognized pre-linked alts.
            )                 # End negative lookahead assertion.
            [?=&+%\w-]*       # Consume any URL (query) remainder.",
           RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private const string ContentFormat = "<iframe class=\"youtubeframe\" width=\"{0}\" height=\"{1}\" src=\"https://www.youtube.com/embed/{2}\" frameborder=\"0\" allowfullscreen></iframe>";

        public override Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobName, int index, CancellationToken cancelToken = default(CancellationToken))
        {
            var match = YoutubeRegex.Match(blobName.AbsoluteUri);
            if (match.Groups.Count < 2 || string.IsNullOrEmpty(match.Groups[1].Value))
            {
                var blobsNamesInCache = new List<string>
            {
                "https://az779114.vo.msecnd.net/preview/" + WebUtility.UrlEncode( blobName.AbsoluteUri) + ".jpg?width=1024&height=768"
            };
                return Task.FromResult(new PreviewResult { ViewName = "Image", Content = blobsNamesInCache });
            }

            var videoId = match.Groups[1].Value;
            return Task.FromResult(new PreviewResult { Content = new List<string> { string.Format(ContentFormat, 800, 450, videoId) } });
        }

        public override bool CanProcessFile(Uri blobName)
        {
            return Domains.Any(d => blobName.AbsoluteUri.StartsWith(d, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> Domains
        {
            get
            {
                yield return "http://www.youtube.com";
                yield return "https://www.youtube.com";
                yield return "http://youtu.be";
                yield return "https://youtu.be";
            }
        }

        //public Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        //{
        //    return Task.FromResult<PreProcessFileResult>(null);
        //}
    }
}
