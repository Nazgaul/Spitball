using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;

namespace Cloudents.Infrastructure.Converters
{
    public class SuggestConverter : ITypeConverter<Suggestions.SuggestionsObject,IEnumerable<string>>
    {
        public IEnumerable<string> Convert(Suggestions.SuggestionsObject source, IEnumerable<string> destination, ResolutionContext context)
        {
            return source.suggestionGroups.FirstOrDefault()?.searchSuggestions.Select(s => s.displayText);
        }
    }
}
