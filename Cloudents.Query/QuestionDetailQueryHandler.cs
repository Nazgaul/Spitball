using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Storage;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Injected")]
    public class QuestionDetailQueryHandler : IQueryHandler<QuestionDataByIdQuery, QuestionDetailDto>
    {
        private readonly ISession _session;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly IBlobProvider _blobProvider2;

        public QuestionDetailQueryHandler(QuerySession session, IBlobProvider<QuestionAnswerContainer> blobProvider, IBlobProvider blobProvider2)
        {
            _session = session.Session;
            _blobProvider = blobProvider;
            _blobProvider2 = blobProvider2;
        }

        private async Task<QuestionDetailDto> GetFromDbAsync(long id, CancellationToken token)
        {
            //TODO: this is left join query need to fix that
            var questionFuture = _session.Query<Question>()
                .Where(w => w.Id == id && w.Item.State == ItemState.Ok)
                .Fetch(f => f.User)
                .Select(s => new QuestionDetailDto(
                    new UserDto(s.User.Id, s.User.Name, s.User.Score),
                //{
                //    Id = s.User.Id,
                //    Name = s.User.Name,
                //    Image = s.User.Image,
                //    Score = s.User.Score
                //},
                    s.Id, s.Text, s.Price, s.Updated, s.CorrectAnswer.Id,
                    s.Color, s.Subject, s.Language, s.Item.VoteCount)
                ).ToFutureValue();
            var answersFuture = _session.Query<Answer>()
                .Where(w => w.Question.Id == id && w.Item.State == ItemState.Ok)
                .Fetch(f => f.User)
                
                //.ThenByDescending(x => x.Item.VoteCount)
                //.ThenBy(x=>x.Id)
                .Select(s => new QuestionDetailAnswerDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Create = s.Created,
                    Vote = new VoteDto()
                    {
                        Votes = s.Item.VoteCount
                    },
                    User = new UserDto(s.User.Id, s.User.Name, s.User.Score)
                    //{
                    //    Id = s.User.Id,
                    //    Name = s.User.Name,
                    //    Image = s.User.Image,
                    //    Score = s.User.Score
                    //}
                }).ToFuture();

            var dto = await questionFuture.GetValueAsync(token);
            if (dto == null)
            {
                return null;
            }
            var answerResult = await answersFuture.GetEnumerableAsync(token);


            dto.Answers = answerResult.OrderByDescending(x => x.Id == dto.CorrectAnswerId)
                .ThenByDescending(x => x.Vote.Votes).ThenBy(x => x.Create);

            return dto;
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
            });

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