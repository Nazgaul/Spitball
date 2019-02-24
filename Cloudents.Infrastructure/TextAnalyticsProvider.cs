using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1.Model;
using IBM.WatsonDeveloperCloud.Util;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static Cloudents.Core.Entities.Language;

namespace Cloudents.Infrastructure
{
    public class TextAnalysisProvider : ITextAnalysis, ITextClassifier
    {
        private class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                //TODO: configuration file ????
                request.Headers.Add("Ocp-Apim-Subscription-Key", "d5a65fba156a490281577c13cbefee2c");
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        private readonly ITextAnalyticsClient _client;
        public TextAnalysisProvider()
        {

            _client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = "https://northeurope.api.cognitive.microsoft.com"
            };
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


            var b = new BatchInput(texts.Where(w => !string.IsNullOrEmpty(w.Value))
                .Select(s => new Input(s.Key.ToString(), s.Value.Truncate(4000)))
                .ToList());
            if (b.Documents.Count == 0)
            {
                return null;
            }
            var result =
                await _client.DetectLanguageAsync(b, cancellationToken: token);



            return result.Documents.Select(x =>
             {
                 CultureInfo t = English;
                 if (x.DetectedLanguages[0].Score > 0)
                 {
                     t = new CultureInfo(x.DetectedLanguages[0].Iso6391Name.Replace("_", "-"));
                 }

                 return new KeyValuePair<T, CultureInfo>((T)Convert.ChangeType(x.Id, typeof(T)), t);
             });
        }

        public async Task<CultureInfo> DetectLanguageAsync(string text, CancellationToken token)
        {
            if (string.IsNullOrEmpty(text))
            {
                return English;
            }
            var result = await DetectLanguageAsync(new[] { new KeyValuePair<string, string>("1", text) }, token);
            return result.FirstOrDefault().Value ?? English;
        }

        public async Task<IEnumerable<string>> KeyPhraseAsync(string text, CancellationToken token)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            KeyPhraseBatchResult result2 = await _client.KeyPhrasesAsync(new MultiLanguageBatchInput(
                new List<MultiLanguageInput>()
                {
                    new MultiLanguageInput(language:"en", text: text.Truncate(4500),id:"1")
                }), cancellationToken: token);

            // Printing keyphrases
            var p = result2.Documents.FirstOrDefault();
            return p?.KeyPhrases.Take(50);

            //foreach (var document in result2.Documents)
            //{
            //    Console.WriteLine($"Document ID: {document.Id} ");

            //    Console.WriteLine("\t Key phrases:");

            //    foreach (string keyphrase in document.KeyPhrases)
            //    {

            //        Console.WriteLine($"\t\t{keyphrase}");
            //    }
            //}
        }
    }

    public class TextClassifierAnalysis : ITextClassifier
    {
        private readonly NaturalLanguageUnderstandingService _naturalLanguageUnderstandingService;
        public TextClassifierAnalysis()
        {
            TokenOptions iamAssistantTokenOptions = new TokenOptions()
            {
                IamApiKey = "tJLg4zulbTIXu1zUprqfDP6KuHpnpW8oQ4iVt06i1nES",
                ServiceUrl = "https://gateway-lon.watsonplatform.net/natural-language-understanding/api"
            };
            _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService(iamAssistantTokenOptions, "2019-02-24");

        }

        public Task<IEnumerable<string>> KeyPhraseAsync(string text, CancellationToken token)
        {
            Parameters parameters = new Parameters()
            {
                Text = text,
                Features = new Features()
                {
                    Keywords = new KeywordsOptions()
                    {
                        Limit = 50,
                        Sentiment = false,
                        Emotion = false
                    },
                    Categories = new CategoriesOptions()
                    {
                        Limit = 50
                    }
                }
            };
            var apiResult = _naturalLanguageUnderstandingService.Analyze(parameters);
            var result = apiResult.Categories.SelectMany(s => s.Label.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries));
            return Task.FromResult<IEnumerable<string>>(result);

        }
    }

    public class TextTranslatorProvider : ITextTranslator
    {
        private readonly IRestClient _client;
        readonly string host = "https://api.cognitive.microsofttranslator.com";
        readonly string route = "/translate";
        readonly string subscriptionKey = "5cbdcfa258c94eacbd4e7ee539b66fda";

        public TextTranslatorProvider(IRestClient client)
        {
            _client = client;
        }


        public async Task<string> TranslateAsync(string text, string to, CancellationToken token)
        {
            var uriBuilder = new UriBuilder(new Uri(host + route));
            uriBuilder.AddQuery(new NameValueCollection()
            {
                ["to"] = to,
                ["api-version"] = "3.0"
            });

            var result = await _client.PostJsonAsync<object, IList<Result>>(uriBuilder.Uri, new[]
            {
                new {Text = text.Truncate(5000)}
            }, new[]
            {
                new KeyValuePair<string, string>("Ocp-Apim-Subscription-Key", subscriptionKey)
            }, token);

            return result.FirstOrDefault()?.translations.FirstOrDefault()?.text;








        }

        private class Result
        {
            public Detectedlanguage detectedLanguage { get; set; }
            public Translation[] translations { get; set; }
        }

        private class Detectedlanguage
        {
            public string language { get; set; }
            public float score { get; set; }
        }

        private class Translation
        {
            public string text { get; set; }
            public string to { get; set; }
        }

    }
}