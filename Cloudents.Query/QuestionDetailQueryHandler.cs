using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Storage;
using Cloudents.Query.Query;
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Injected")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    public class QuestionDetailQueryHandler : IQueryHandler<QuestionDataByIdQuery, QuestionDetailDto>
    {
        private class QuestionFlatDto
        {
            [DtoToEntityConnection(nameof(Question.User.Id))]
            public long UserId { get; private set; }
            [DtoToEntityConnection(nameof(Question.User.Name))]
            public string UserName { get; set; }
            [DtoToEntityConnection(nameof(Question.User.Score))]
            public int UserScore { get; set; }
            [DtoToEntityConnection(nameof(Question.Id))]
            public long Id { get; set; }
            [DtoToEntityConnection(nameof(Question.Text))]
            public string Text { get; set; }
            [DtoToEntityConnection(nameof(Question.Price))]
            public decimal Price { get; set; }
            [DtoToEntityConnection(nameof(Question.Updated))]
            public DateTime Create { get; set; }
            [DtoToEntityConnection(nameof(Question.CorrectAnswer.Id))]
            public Guid? CorrectAnswerId { get; set; }
            [DtoToEntityConnection(nameof(Question.Subject))]
            public QuestionSubject? Subject { get; set; }
            [DtoToEntityConnection(nameof(Question.Language))]
            public CultureInfo Language { get; set; }
            [DtoToEntityConnection(nameof(Question.VoteCount))]
            public int Votes { get; set; }
            [DtoToEntityConnection(nameof(Question.Course))]
            public string Course { get; set; }
        }

        private class AnswerFlatDto
        {
            [DtoToEntityConnection(nameof(Answer.Id))]
            public Guid Id { get; set; }
            [DtoToEntityConnection(nameof(Answer.Text))]
            public string Text { get; set; }
            [DtoToEntityConnection(nameof(Answer.User.Id))]
            public long UserId { get; set; }
            [DtoToEntityConnection(nameof(Answer.User.Name))]
            public string UserName { get; set; }
            [DtoToEntityConnection(nameof(Answer.User.Score))]
            public int UserScore { get; set; }
            [DtoToEntityConnection(nameof(Answer.Created))]
            public DateTime Created { get; set; }
            [DtoToEntityConnection(nameof(Answer.VoteCount))]
            public int VoteCount { get; set; }
            [DtoToEntityConnection(nameof(Answer.Language))]
            public CultureInfo Language { get; set; }
        }

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
                using (var grid = connection.QueryMultiple(@"
                        select U.Id as UserId, U.Name as UserName, U.Score as UserScore, 
                                                        Q.Id, Q.Text,  Q.Price, 
	                                                    Q.Updated as 'Create', Q.CorrectAnswer_id as CorrectAnswerId, 
                                                        Q.Subject_id as Subject, 
	                                                    Q.Language, Q.VoteCount as Votes, Q.CourseId as Course
                                                    from sb.Question Q
                                                    join sb.[user] U
	                                                    on Q.UserId = U.Id
                                                    where Q.Id = @id and Q.State = 'ok';

                                                    select A.Id, A.Text, U.Id as UserId, U.Name as UserName, 
                                                    U.Score as UserScore, A.Created, A.VoteCount, A.Language
                                                    from sb.Answer A
                                                    join sb.[user] U
	                                                    on A.UserId = U.Id
                                                    where A.State = 'ok' and A.QuestionId = @id;", new { id }))
                {



                    var res = await grid.ReadSingleOrDefaultAsync<QuestionFlatDto>();
                    if (res == null)
                    {
                        return null;
                    }

                    var questionDetailDto = new QuestionDetailDto
                    {
                        User = new UserDto { Id = res.UserId, Name = res.UserName, Score = res.UserScore },
                        Course = res.Course,
                        Vote = new VoteDto { Votes = res.Votes },
                        Price = res.Price,
                        Id = res.Id,
                        Subject = res.Subject,
                        CorrectAnswerId = res.CorrectAnswerId,
                        Create = res.Create,
                        Text = res.Text,
                        IsRtl = res.Language?.TextInfo.IsRightToLeft ?? false
                    };

                    var answers = await grid.ReadAsync<AnswerFlatDto>();

                    questionDetailDto.Answers = answers
                        .OrderByDescending(x => x.Id == questionDetailDto.CorrectAnswerId)
                        .ThenByDescending(x => x.VoteCount).ThenBy(x => x.Created).Select(a =>
                            new QuestionDetailAnswerDto
                            {
                                User = new UserDto { Id = a.UserId, Name = a.UserName, Score = a.UserScore },
                                Id = a.Id,
                                Vote = new VoteDto { Votes = a.VoteCount },
                                Text = a.Text,
                                Create = a.Created,
                                IsRtl = a.Language?.TextInfo.IsRightToLeft ?? false
                            });


                    return questionDetailDto;
                }
            }, token);

            return questionDetailResult;


        }

        public async Task<QuestionDetailDto> GetAsync(QuestionDataByIdQuery query, CancellationToken token)
        {
            var dtoTask = GetFromDbAsync(query.Id, token);

            //TODO: this is left join query need to fix that

            var filesTask = _blobProvider.FilesInDirectoryAsync($"{query.Id}", token);
            await Task.WhenAll(dtoTask, filesTask);
            var files = filesTask.Result.Select(s => _blobProvider2.GeneratePreviewLink(s, TimeSpan.FromMinutes(20)));
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




}