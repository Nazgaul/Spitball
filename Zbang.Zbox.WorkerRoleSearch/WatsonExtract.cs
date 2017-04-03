﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlchemyAPIClient;
using AlchemyAPIClient.Requests;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class WatsonExtract : IWatsonExtract
    {
        private readonly AlchemyClient m_Client =
              new AlchemyClient("e05317b0a67d8a3d0bf82f5b0e0b58012b717779");

        public async Task<IEnumerable<string>> GetConceptAsync(string text, CancellationToken token)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            var request = new AlchemyTextConceptsRequest(text, m_Client)
            {
                KnowledgeGraph = true,
                LinkedData = true,
                ShowSourceText = true,
                MaxRetrieve = 30
            };
            try
            {
                var result = await request.GetResponse(token).ConfigureAwait(false);
                int dummy;
                return result.Concepts.Where(w => w.Relevance > 0.75).Select(s => s.Text).Where(w => !int.TryParse(w, out dummy));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("watson concept error" + text, ex);
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetKeywordAsync(string text, CancellationToken token)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            
            
            var request = new AlchemyTextKeywordsRequest(text, m_Client)
            {
                KnowledgeGraph = true,
                ShowSourceText = true,
                Sentiment = false,
                MaxRetrieve = 30
            };

            try
            {
                var result = await request.GetResponse(token).ConfigureAwait(false);
                int dummy;
                
                return result.Keywords.Where(w => w.Relevance > 0.75).Select(s => s.Text).Where(w => !int.TryParse(w, out dummy));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("watson keyword error" + text, ex);
                return null;
            }
        }

        public async Task<Language> GetLanguageAsync(string text, CancellationToken token)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Language.Undefined;
            }


            var request = new AlchemyTextLanguageRequest(text, m_Client);
            

            try
            {
                var result = await request.GetResponse(token).ConfigureAwait(false);
                if (result.Iso_639_3 == "eng")
                {
                    return Language.EnglishUs;
                }
                if (result.Iso_639_3 == "heb")
                {
                    return Language.Hebrew;
                }
                return Language.Undefined;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("watson text error" + text, ex);
               
            }
            return Language.Undefined;
        }

    }

    public interface IWatsonExtract
    {
        Task<IEnumerable<string>> GetConceptAsync(string text, CancellationToken token);
        Task<IEnumerable<string>> GetKeywordAsync(string text, CancellationToken token);
        Task<Language> GetLanguageAsync(string text, CancellationToken token);
    }
}
