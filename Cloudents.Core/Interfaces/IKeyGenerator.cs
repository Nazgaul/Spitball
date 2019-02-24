﻿using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IKeyGenerator
    {
        string GenerateKey(object sourceObject);
    }

    public interface ITextAnalysis
    {
        Task<IEnumerable<KeyValuePair<T, CultureInfo>>> DetectLanguageAsync<T>(
            IEnumerable<KeyValuePair<T, string>> texts, CancellationToken token);

        Task<CultureInfo> DetectLanguageAsync(string text, CancellationToken token);


       
    }

    public interface ITextClassifier
    {
        Task<IEnumerable<string>> KeyPhraseAsync(string text, CancellationToken token);
    }

    public interface ITextTranslator
    {
        Task<string> TranslateAsync(string text, string to, CancellationToken token);
    }
}