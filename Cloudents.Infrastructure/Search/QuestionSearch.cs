namespace Cloudents.Infrastructure.Search
{
    //public class QuestionSearch //: IQuestionSearch
    //{
    //    private readonly ISearchIndexClient _client;
    //    private readonly IMapper _mapper;

    //    public QuestionSearch(ISearchServiceClient client, string indexName, IMapper mapper)
    //    {
    //        _mapper = mapper;
    //        _client = client.Indexes.GetClient(indexName);
    //    }

    //    public async Task<ResultWithFacetDto<QuestionDto>> SearchAsync(QuestionsQuery query, CancellationToken token)
    //    {
    //        string filterStr = null;

    //        if (query.Source != null)
    //        {
    //            filterStr = string.Join(" or ", query.Source.Select(s =>
    //                $"{nameof(Question.Subject)} eq '{s}'"));
    //        }

    //        var searchParameter = new SearchParameters
    //        {
    //            Facets = new[] { nameof(Question.Subject) },
    //            Filter = filterStr,
    //            Top = 50,
    //            Skip = query.Page * 50
    //        };

    //        var result = await
    //            _client.Documents.SearchAsync<Question>(query.Term, searchParameter,
    //                cancellationToken: token).ConfigureAwait(false);

    //        var retVal = new ResultWithFacetDto<QuestionDto>
    //        {
    //            Result = _mapper.Map<IEnumerable<QuestionDto>>(result.Results.Select(s => s.Document))
    //        };
    //        if (result.Facets.TryGetValue(nameof(Question.Subject), out var p))
    //        {
    //            retVal.Facet = p.Select(s => s.AsValueFacetResult<string>().Value);
    //        }
    //        return retVal;
    //    }
    //}
}