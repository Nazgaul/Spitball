﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Storage;
using Cloudents.Infrastructure.Data;
using Cloudents.Query.Query;
using Dapper;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Injected")]
    public class QuestionDetailQueryHandler : IQueryHandler<QuestionDataByIdQuery, QuestionDetailDto>
    {
        private readonly DapperRepository _dapper;

        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly IBlobProvider _blobProvider2;

        public QuestionDetailQueryHandler(DapperRepository dapper, IBlobProvider<QuestionAnswerContainer> blobProvider, IBlobProvider blobProvider2)
        {
            _dapper = dapper;
            _blobProvider = blobProvider;
            _blobProvider2 = blobProvider2;
        }

        private async Task<QuestionDetailDto> GetFromDbAsync(long id, CancellationToken token)
        {


            var questionDetailResult = await _dapper.WithConnectionAsync(async connection =>
            {
                var grid = connection.QueryMultiple(@"select U.Id as UserId, U.Name as UserName, U.Score as UserScore, 
                                                        Q.Id, Q.Text,  Q.Price, 
	                                                    Q.Updated as 'Create', Q.CorrectAnswer_id as CorrectAnswerId, 
                                                        Q.color as Color, Q.Subject_id as Subject, 
	                                                    Q.Language, Q.VoteCount as Votes, Q.CourseId as Course
                                                    from sb.Question Q
                                                    join sb.[user] U
	                                                    on Q.UserId = U.Id
                                                    where Q.Id = @id;

                                                    select A.Id, A.Text, U.Id as UserId, U.Name as UserName, 
                                                    U.Score as UserScore, A.Created, A.VoteCount, A.Language
                                                    from sb.Answer A
                                                    join sb.[user] U
	                                                    on A.UserId = U.Id
                                                    where A.State = 'ok' and A.QuestionId = @id;", new { id = id });



                var res = await grid.ReadFirstAsync<QuestionDetailQueryFlatDto>();

                var questionDetailDto = new QuestionDetailDto(
                     new UserDto(res.UserId, res.UserName, res.UserScore),
                    res.Id, res.Text, res.Price, res.Create, res.CorrectAnswerId, res.Color, res.Subject,
                    new CultureInfo(res.Language), res.Votes, res.Course
                    );

                var answers = await grid.ReadAsync<QuestionDetailAnswerFlatDto>();

              
                foreach (var a in answers)
                {
                    questionDetailDto.Answers.Add(new QuestionDetailAnswerDto(a.Id, a.Text,
                    a.UserId, a.UserName, a.UserScore, a.Created,
                    a.VoteCount, new CultureInfo(a.Language)));
                }
             


                return questionDetailDto;
            }, token);

            return questionDetailResult;

        }

        public async Task<QuestionDetailDto> GetAsync(QuestionDataByIdQuery query, CancellationToken token)
        {
            var dtoTask = GetFromDbAsync(query.Id, token);

            //TODO: this is left join query need to fix that

            var filesTask = _blobProvider.FilesInDirectoryAsync($"{query.Id}", token);
            await Task.WhenAll(dtoTask, filesTask);
            var files = filesTask.Result.Select(s => _blobProvider2.GeneratePreviewLink(s, 20));
            var dto = dtoTask.Result;

            if (dto == null)
            {
                return null;
            }
            //TODO should not be here
            var aggregateFiles = AggregateFiles(files);
            dto.Files = aggregateFiles[null];
            dto.Answers = dto.Answers.Select(s =>
            {
                s.Files = aggregateFiles[s.Id];
                return s;
            }).ToList();

            return dto;
        }

        private static ILookup<Guid?, Uri> AggregateFiles(IEnumerable<Uri> files)
        {
            var aggregateFiles = files.ToLookup<Uri, Guid?>(v =>
            {
                if (v.Segments.Length == 5)
                {
                    return null;
                }

                return Guid.Parse(v.Segments[5].Replace("/", string.Empty));
            });
            return aggregateFiles;
        }
    }


    public class QuestionDetailAnswerFlatDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int UserScore { get; set; }
        public DateTime Created { get; set; }
        public int VoteCount { get; set; }
        public string Language { get; set; }
    }

}