﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Search.Question;

namespace Cloudents.Infrastructure.Search
{
    public class QuestionSearch : IQuestionSearch
    {
        private readonly AzureQuestionSearch _questionSearch;
        private readonly IQueryBus _queryBus;

        public QuestionSearch(AzureQuestionSearch questionSearch, IQueryBus queryBus)
        {
            _questionSearch = questionSearch;
            _queryBus = queryBus;
        }

        public async Task<QuestionWithFacetDto> SearchAsync(QuestionsQuery query, CancellationToken token)
        {
            var sw = new Stopwatch();
            sw.Start();
            var searchResult = await _questionSearch.SearchAsync(query, token);
            sw.Stop();
            Console.WriteLine(sw.ElapsedTicks);
            var ids = searchResult.Results.Select(s => long.Parse(s.Document.Id));
            var queryDb = new QuestionsByIdsQuery(ids);
            sw.Restart();
            var dbResult = await _queryBus.QueryAsync(queryDb, token);
            sw.Stop();
            Console.WriteLine(sw.ElapsedTicks);

            var retVal = new QuestionWithFacetDto {Result = dbResult};

            //{
            //    //Result = result.Results.Select(s => new QuestionDto()
            //    //{

            //    //    User = new UserDto
            //    //    {
            //    //        Id = s.Document.UserId,
            //    //        Name = s.Document.UserName,
            //    //        Image = s.Document.UserImage
            //    //    },
            //    //    Id = long.Parse(s.Document.Id),
            //    //    DateTime = s.Document.DateTime,
            //    //    Answers = s.Document.AnswerCount,
            //    //    Subject = s.Document.Subject,
            //    //    Color = s.Document.Color,
            //    //    Files = s.Document.FilesCount,
            //    //    HasCorrectAnswer = s.Document.HasCorrectAnswer,
            //    //    Price = (decimal)s.Document.Price,
            //    //    Text = s.Document.Text
            //    //})
            //};
            if (searchResult.Facets.TryGetValue(nameof(Core.Entities.Search.Question.Subject), out var p))
            {
                retVal.FacetSubject = p.Select(s => (QuestionSubject)s.AsValueFacetResult<long>().Value);
            }

            if (searchResult.Facets.TryGetValue(nameof(Core.Entities.Search.Question.State), out var p2))
            {
                retVal.FacetState = p2.Select(s => (QuestionFilter)s.AsValueFacetResult<long>().Value);
            }
            return retVal;
            //QuestionsByIdsQuery
        }
    }
}