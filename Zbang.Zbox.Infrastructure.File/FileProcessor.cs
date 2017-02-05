using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SimMetricsMetricUtilities;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{

    public abstract class FileProcessor : ContentProcessor, IContentProcessor
    {

        protected const int ThumbnailWidth = 148;
        protected const int ThumbnailHeight = 187;

        protected const string CacheVersionPrefix = "V";
        protected const string PagesInDocsMetaKey = "pageCount";


        protected readonly IBlobProvider BlobProvider;


        protected FileProcessor(IBlobProvider blobProvider)
        {
            BlobProvider = blobProvider;
        }



        protected string GetBlobNameFromUri(Uri blobUri)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            return blobName;
        }


        public abstract Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken));

        public abstract Task<string> ExtractContentAsync(Uri blobUri,
            CancellationToken cancelToken = default(CancellationToken));

        protected string StripUnwantedChars(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var sentenceRegex = new Regex("[^.!?;]*[^.?!;]*[.?!;]", RegexOptions.Compiled, TimeSpan.FromSeconds(10));
            var pageTexts = new List<string>();
            string result = null;
            try
            {
               
                var d = sentenceRegex.Matches(input);
                if (d.Count == 0)
                {
                    foreach (var sentence in SplitSentence(input))
                    {
                        AddSentenceToList(sentence, pageTexts);
                    }
                }
                foreach (Match match in d)
                {
                    var x = SplitSentence(match.Value);

                    foreach (var t in x)
                    {
                        AddSentenceToList(t, pageTexts);
                    }
                }
                result = string.Join(" ", pageTexts);
            }
            catch (RegexMatchTimeoutException)
            {
                result = input;
            }
            
            var eightOrNineDigitsId = new Regex(@"\b\d{8,9}\b", RegexOptions.Compiled);
            result = TextManipulation.SpaceReg.Replace(result, " ");
            result = eightOrNineDigitsId.Replace(result, string.Empty);
            result = new string(result.Where(w => char.IsLetterOrDigit(w) || char.IsWhiteSpace(w)).ToArray());
            //result = result.Replace("\0", string.Empty);
            result = result.Replace("בס\"ד", string.Empty);
            result = result.Replace("find more resources at oneclass.com", string.Empty);
            return result;
        }

        private static IEnumerable<string> SplitSentence(string input)
        {
            return input.Split(new[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void AddSentenceToList(string t, List<string> pageTexts)
        {
            var jaroWinkler = new JaroWinkler();
            var z = TextManipulation.SpaceReg.Split(t);
            if (!z.Any(a => a.Length > 1)) return;
            var result = jaroWinkler.BatchCompareSet(pageTexts.ToArray(), t);
            if (result.Any(w => w > 0.95))
            {
                return;
            }
            pageTexts.Add(t);
        }

        protected async Task UploadMetaDataAsync(
            Uri blobUri,
            int pageCount,
            string getCacheVersionPrefix, CancellationToken token
            )
        {
            var metaData = await BlobProvider.FetchBlobmetaDataAsync(blobUri, token) ?? new Dictionary<string, string>();
            metaData = RemoveOldMetaTags(metaData, getCacheVersionPrefix);
            metaData[PagesInDocsMetaKey] = pageCount.ToString();
            await BlobProvider.SaveMetaDataToBlobAsync(blobUri, metaData, token);
        }

        protected IDictionary<string, string> RemoveOldMetaTags(IDictionary<string, string> metaTags, string cacheVersionPrfix)
        {
            var oldElements = metaTags.Where(w =>
                Regex.IsMatch(w.Key, @"\d") && !w.Key.StartsWith(cacheVersionPrfix)).Select(s => s.Key).ToList();

            foreach (var oldElement in oldElements)
            {
                metaTags.Remove(oldElement);
            }

            var z = metaTags.Select(s => s.Key).Where(w => w.StartsWith(cacheVersionPrfix)).Select(s =>
            {
                var number = s.Remove(0, cacheVersionPrfix.Length);
                return Convert.ToInt32(number);
            }).Where(p => p >= 15).ToList();

            foreach (int i in z)
            {
                metaTags.Remove(cacheVersionPrfix + i);
            }


            return metaTags;
        }


    }
}
