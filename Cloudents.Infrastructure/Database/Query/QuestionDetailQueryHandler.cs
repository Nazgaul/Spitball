﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Injected")]
    public class QuestionDetailQueryHandler: IQueryHandler<QuestionDataByIdQuery, QuestionDetailDto>
    {
        private readonly ISession _session;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;

        public QuestionDetailQueryHandler(ReadonlySession session, IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _session = session.Session;
            _blobProvider = blobProvider;
        }

        private async Task<QuestionDetailDto> GetFromDbAsync(long id, CancellationToken token)
        {
            //TODO: this is left join query need to fix that
            var questionFuture = _session.Query<Question>().Where(w => w.Id == id)
                .Fetch(f => f.User)
                .Select(s => new QuestionDetailDto
                {
                    User = new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image
                    },
                    Id = s.Id,
                    Create = s.Updated,
                    Price = s.Price,
                    Subject = s.Subject,
                    Text = s.Text,
                    Color = s.Color,
                    CorrectAnswerId = s.CorrectAnswer.Id
                }).ToFutureValue();
            var answersFuture = _session.Query<Answer>()
                .Where(w => w.Question.Id == id)
                .Fetch(f => f.User)
                .Select(s => new QuestionDetailAnswerDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Create = s.Created,
                    User = new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image
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

            var filesTask = _blobProvider.FilesInDirectoryAsync($"question/{query.Id}", token);
            await Task.WhenAll(dtoTask, filesTask).ConfigureAwait(false);
            var files = filesTask.Result;
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