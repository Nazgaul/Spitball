using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;

namespace Cloudents.Infrastructure
{
    //public class WatsonTextAnalysisProvider : ITextAnalysis
    //{
    //    readonly LanguageTranslatorService _languageTranslator;
    //    public TextAnalysisProvider()
    //    {

    //        _languageTranslator = new LanguageTranslatorService(new TokenOptions()
    //        {
    //            IamApiKey = "y4dcd2v_eK-H4qkrJ8gedQIK7AZDavY1K-5TpQYiItkg",
    //            ServiceUrl = "https://gateway-lon.watsonplatform.net/language-translator/api"
    //        }, "2019-02-24");

    //    }

    //    [ItemCanBeNull]
    //    public Task<CultureInfo> DetectLanguageAsync(string text, CancellationToken token)
    //    {
    //        var result2 = _languageTranslator.Identify(text.Truncate(5000));
    //        var identify = result2.Languages.FirstOrDefault();
    //        if (identify == null)
    //        {
    //            return Task.FromResult<CultureInfo>(null);
    //        }
    //        return Task.FromResult(new CultureInfo(identify.Language));

    //    }


    //}


    public class AzureTextAnalysisProvider : ITextAnalysis
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
        public AzureTextAnalysisProvider()
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
        private async Task<IEnumerable<KeyValuePair<T, CultureInfo>>> DetectLanguageAsync<T>(IEnumerable<KeyValuePair<T, string>> texts, CancellationToken token)
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
                CultureInfo t = null;
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
                return null;
            }
            var result = await DetectLanguageAsync(new[] { new KeyValuePair<string, string>("1", text) }, token);
            return result.FirstOrDefault().Value ;
        }
      
    }

    //public class TextClassifierAnalysis : ITextClassifier
    //{
    //    private readonly NaturalLanguageUnderstandingService _naturalLanguageUnderstandingService;
    //    public TextClassifierAnalysis()
    //    {
    //        TokenOptions iamAssistantTokenOptions = new TokenOptions()
    //        {
    //            IamApiKey = "tJLg4zulbTIXu1zUprqfDP6KuHpnpW8oQ4iVt06i1nES",
    //            ServiceUrl = "https://gateway-lon.watsonplatform.net/natural-language-understanding/api"
    //        };
    //        _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService(iamAssistantTokenOptions,
    //            "2019-02-24");

    //    }

    //    public Task<IEnumerable<string>> KeyPhraseAsync(string text, CancellationToken token)
    //    {
    //        Parameters parameters = new Parameters()
    //        {
    //            Text = text,
    //            Features = new Features()
    //            {
    //                //Keywords = new KeywordsOptions()
    //                //{
    //                //    Limit = 50,
    //                //    Sentiment = false,
    //                //    Emotion = false
    //                //},
    //                Categories = new CategoriesOptions()
    //                {
    //                    Limit = 50
    //                },

    //            }
    //        };
    //        var apiResult = _naturalLanguageUnderstandingService.Analyze(parameters);
    //        var result = apiResult.Categories.SelectMany(s => s.Label.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries));
    //        return Task.FromResult(result);

    //    }
    //}

    //public class TextTranslatorProvider : ITextTranslator
    //{
    //    private readonly IRestClient _client;
    //    readonly string host = "https://api.cognitive.microsofttranslator.com";
    //    readonly string route = "/translate";
    //    readonly string subscriptionKey = "5cbdcfa258c94eacbd4e7ee539b66fda";

    //    public TextTranslatorProvider(IRestClient client)
    //    {
    //        _client = client;
    //    }


    //    public async Task<string> TranslateAsync(string text, string from, string to, CancellationToken token)
    //    {
    //        var uriBuilder = new UriBuilder(new Uri(host + route));
    //        uriBuilder.AddQuery(new NameValueCollection()
    //        {
    //            ["to"] = to,
    //            ["from"] = from,
    //            ["api-version"] = "3.0"
    //        });

    //        var result = await _client.PostJsonAsync<object, IList<Result>>(uriBuilder.Uri, new[]
    //        {
    //            new {Text = text.Truncate(5000)}
    //        }, new[]
    //        {
    //            new KeyValuePair<string, string>("Ocp-Apim-Subscription-Key", subscriptionKey)
    //        }, token);

    //        return result.FirstOrDefault()?.translations.FirstOrDefault()?.text;
    //    }

    //    private class Result
    //    {
    //        public Detectedlanguage detectedLanguage { get; set; }
    //        public Translation[] translations { get; set; }
    //    }

    //    private class Detectedlanguage
    //    {
    //        public string language { get; set; }
    //        public float score { get; set; }
    //    }

    //    private class Translation
    //    {
    //        public string text { get; set; }
    //        public string to { get; set; }
    //    }

    //}
}