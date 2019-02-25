using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using IBM.WatsonDeveloperCloud.LanguageTranslator.v3;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1.Model;
using IBM.WatsonDeveloperCloud.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure
{
    public class TextAnalysisProvider : ITextAnalysis
    {
        readonly LanguageTranslatorService _languageTranslator;
        public TextAnalysisProvider()
        {

            _languageTranslator = new LanguageTranslatorService(new TokenOptions()
            {
                IamApiKey = "y4dcd2v_eK-H4qkrJ8gedQIK7AZDavY1K-5TpQYiItkg",
                ServiceUrl = "https://gateway-lon.watsonplatform.net/language-translator/api"
            }, "2019-02-24");

        }

        [ItemCanBeNull]
        public Task<CultureInfo> DetectLanguageAsync(string text, CancellationToken token)
        {
            var result2 = _languageTranslator.Identify(text.Truncate(5000));
            var identify = result2.Languages.FirstOrDefault();
            if (identify == null)
            {
                return Task.FromResult<CultureInfo>(null);
            }
            return Task.FromResult(new CultureInfo(identify.Language));

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
            _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService(iamAssistantTokenOptions,
                "2019-02-24");

        }

        public Task<IEnumerable<string>> KeyPhraseAsync(string text, CancellationToken token)
        {
            Parameters parameters = new Parameters()
            {
                Text = text,
                Features = new Features()
                {
                    //Keywords = new KeywordsOptions()
                    //{
                    //    Limit = 50,
                    //    Sentiment = false,
                    //    Emotion = false
                    //},
                    Categories = new CategoriesOptions()
                    {
                        Limit = 50
                    },

                }
            };
            var apiResult = _naturalLanguageUnderstandingService.Analyze(parameters);
            var result = apiResult.Categories.SelectMany(s => s.Label.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries));
            return Task.FromResult(result);

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


        public async Task<string> TranslateAsync(string text,string from, string to, CancellationToken token)
        {
            var uriBuilder = new UriBuilder(new Uri(host + route));
            uriBuilder.AddQuery(new NameValueCollection()
            {
                ["to"] = to,
                ["from"] = from,
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