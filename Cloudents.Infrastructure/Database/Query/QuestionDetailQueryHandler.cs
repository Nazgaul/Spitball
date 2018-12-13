using Cloudents.Core.DTOs;
using Cloudents.Domain.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

namespace Cloudents.Infrastructure.Database.Query
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
                .Select(s => new QuestionDetailDto(new UserDto
                {
                    Id = s.User.Id,
                    Name = s.User.Name,
                    Image = s.User.Image,
                    Score = s.User.Score
                }, s.Id, s.Text, s.Price, s.Updated, s.CorrectAnswer.Id,
                    s.Color, s.Subject, s.Language, s.Item.VoteCount)
                ).ToFutureValue();
            var answersFuture = _session.Query<Answer>()
                .Where(w => w.Question.Id == id && w.Item.State == ItemState.Ok)
                .Fetch(f => f.User)
                .Select(s => new QuestionDetailAnswerDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Create = s.Created,
                    Vote = new VoteDto()
                    {
                        Votes = s.Item.VoteCount
                    },
                    User = new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image,
                        Score = s.User.Score
                    }
                }).ToFuture();

            var dto = await questionFuture.GetValueAsync(token).ConfigureAwait(false);
            if (dto == null)
            {
                return null;
            }
            dto.Answers = await answersFuture.GetEnumerableAsync(token).ConfigureAwait(false);

            return dto;
        }

        public async Task<QuestionDetailDto> GetAsync(QuestionDataByIdQuery query, CancellationToken token)
        {
            var dtoTask = GetFromDbAsync(query.Id, token);

            //TODO: this is left join query need to fix that

            var filesTask = _blobProvider.FilesInDirectoryAsync($"{query.Id}", token);
            await Task.WhenAll(dtoTask, filesTask).ConfigureAwait(false);
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