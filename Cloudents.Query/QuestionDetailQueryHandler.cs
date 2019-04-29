using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Storage;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;
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
        private readonly IQuestionsDirectoryBlobProvider _blobProvider;
        private readonly IStatelessSession _session;

        public QuestionDetailQueryHandler(QuerySession session,
            IQuestionsDirectoryBlobProvider blobProvider
           )
        {
            _session = session.StatelessSession;
            _blobProvider = blobProvider;
        }

        private async Task<QuestionDetailDto> GetFromDbAsync(long id, CancellationToken token)
        {

            var questionFuture = _session.Query<Question>()
                .Where(w => w.Id == id && w.Status.State == ItemState.Ok)
                .Fetch(f => f.User)
                .Select(s => new QuestionDetailDto
                {
                    User = new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image,
                        Score = s.User.Score
                    },
                    Id = s.Id,
                    Subject = s.Subject,
                    Course = s.Course.Id,
                    Vote = new VoteDto
                    {
                        Votes = s.VoteCount
                    },
                    Text = s.Text,
                    CorrectAnswerId = s.CorrectAnswer.Id,
                    Create = s.Updated,
                    IsRtl = SetIsRtl(s.Language),

                }
               
                ).ToFutureValue();
            var answersFuture = _session.Query<Answer>()
                .Where(w => w.Question.Id == id && w.Status.State == ItemState.Ok)
                .Fetch(f => f.User)
                .Select(s => new QuestionDetailAnswerDto
                (
                    s.Id,
                    s.Text,
                    new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image,
                        Score = s.User.Score
                    },
                    s.Created,

                    new VoteDto
                    {
                        Votes = s.VoteCount
                    },

                    s.Language


        )).ToFuture();

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

        private static bool SetIsRtl(CultureInfo info)
        {
            return info?.TextInfo.IsRightToLeft ?? false;
        }

        public async Task<QuestionDetailDto> GetAsync(QuestionDataByIdQuery query, CancellationToken token)
        {
            var dtoTask = GetFromDbAsync(query.Id, token);
            var filesTask = _blobProvider.FilesInDirectoryAsync($"{query.Id}", token);
            await Task.WhenAll(dtoTask, filesTask);
            var files = filesTask.Result.Select(s => _blobProvider.GeneratePreviewLink(s, TimeSpan.FromMinutes(20)));
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