﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Search
{
    public class DocumentCseSearch : IDocumentCseSearch
    {
        private readonly ISearch _search;
        private readonly ISearchConvertRepository _searchConvertRepository;

        public DocumentCseSearch(ISearch search, ISearchConvertRepository searchConvertRepository)
        {
            _search = search;
            _searchConvertRepository = searchConvertRepository;
        }

        public async Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token)
        {
            var (universitySynonym, courses) = await _searchConvertRepository.ParseUniversityAndCoursesAsync(model.University, model.Courses, token).ConfigureAwait(false);


            var cseModel = new SearchModel(model.Query, model.Source, model.Page, model.Sort, CustomApiKey.Documents,courses,universitySynonym,"biology",model.DocType);
            //var term = new List<string>();

            //if (universitySynonym != null)
            //{
            //    term.Add(string.Join(" OR ", universitySynonym.Select(s => '"' + s + '"')));
            //}
            //if (courses != null)
            //{
            //    term.Add(string.Join(" OR ", courses.Select(s => '"' + s.UppercaseFirst() + '"')));
            //}
            //if (model.Query != null)
            //{
            //    term.AddNotNull(string.Join(" ", model.Query));
            //}
            //term.AddNotNull(model.DocType);
            //if (term.Count == 0)
            //{
            //    term.Add("biology");
            //}
            //var result = Enumerable.Range(model.Page * CseSearch.NumberOfPagesPerRequest, CseSearch.NumberOfPagesPerRequest).Select(s =>
            //{
            //    var cseModel = new SearchModel(term, model.Source, s, model.Sort, CustomApiKey.Documents);

            //    return _search.DoSearchAsync(cseModel,
            //        token);
            //}).ToList();

            var result = await _search.DoSearchAsync(cseModel, token);

            return new ResultWithFacetDto<SearchResult>
            {
                Result = result,
                Facet = new[]
                {
                    "Spitball.co",
                    "Studysoup.com",
                    "Coursehero.com",
                    "Cliffsnotes.com",
                    "Oneclass.com",
                    "Koofers.com",
                }
            };
        }
    }
}
