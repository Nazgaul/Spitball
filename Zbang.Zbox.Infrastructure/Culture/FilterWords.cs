using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Culture
{
    public class FilterWords : IFilterWords
    {
        private const string CacheKey = "removeWords";
        private const string CacheRegion = "removeWords";
        private readonly ITableProvider m_TableProvider;
        private readonly ICache m_Cache;

        public FilterWords(ITableProvider tableProvider, ICache cacheProvider)
        {
            m_TableProvider = tableProvider;
            m_Cache = cacheProvider;
        }

        public string RemoveWords(string phrase)
        {
            phrase = phrase.Trim();
            if (string.IsNullOrWhiteSpace(phrase))
            {
                return phrase;
            }
            var wordToFilter = GetWordsToFilter().Select(s => s.ToLower()).ToList();
            var sb = new StringBuilder();

            var words = phrase.Split(new[] { " ", "%" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (wordToFilter.Contains(word.ToLower()))
                {
                    continue;
                }
                sb.Append(word + "%");
            }
            return sb.ToString().Trim();
        }

        private IEnumerable<string> GetWordsToFilter()
        {
            var words = m_Cache.GetFromCache(CacheKey, CacheRegion) as IEnumerable<string>;
            if (words != null)
            {
                return words;
            }
            words = m_TableProvider.GetFileterWored().ToList();
            try
            {
                m_Cache.AddToCache(CacheKey, words, TimeSpan.FromHours(1), CacheRegion);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("GetWordsToFilter", ex);
            }
            return words;
        }
    }
}
