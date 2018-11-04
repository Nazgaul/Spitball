﻿using Cloudents.Core.Interfaces;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure
{
    public class TextAnalysisProvider : ITextAnalysis
    {
        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                //TODO: configuration file ????
                request.Headers.Add("Ocp-Apim-Subscription-Key", "d5a65fba156a490281577c13cbefee2c");
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        /// <summary>
        /// Detect batch of sentences
        /// </summary>
        /// <typeparam name="T">The type of the id</typeparam>
        /// <param name="texts">Key value of id - string</param>
        /// <param name="token"></param>
        /// <returns>key value of id - culture</returns>
        public async Task<IEnumerable<KeyValuePair<T, CultureInfo>>> DetectLanguageAsync<T>(IEnumerable<KeyValuePair<T, string>> texts, CancellationToken token)
        {
            ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = "https://northeurope.api.cognitive.microsoft.com"
            };

            var b = new BatchInput(texts.Select(s => new Input(s.Key.ToString(), s.Value))
                .ToList());
            if (b.Documents.Count == 0)
            {
                return null;
            }
            var result =
                await client.DetectLanguageAsync(b, cancellationToken: token);



            return result.Documents.Select(x =>
             {
                 var t = CultureInfo.InvariantCulture;

                 if (x.DetectedLanguages[0].Score.GetValueOrDefault(0).Equals(0))
                 {
                     t = new CultureInfo("en");
                     //x.DetectedLanguages[0] = new DetectedLanguage("English", "en", 1);
                 }
                 else if (x.DetectedLanguages[0].Score > 0)
                 {
                     t = new CultureInfo(x.DetectedLanguages[0].Iso6391Name);
                 }

                 return new KeyValuePair<T, CultureInfo>((T)Convert.ChangeType(x.Id, typeof(T)), t);
             });
        }

        public async Task<CultureInfo> DetectLanguageAsync(string text, CancellationToken token)
        {
            var result = await DetectLanguageAsync(new[] { new KeyValuePair<string, string>("1", text) }, token);
            return result.First().Value;
        }
    }
}