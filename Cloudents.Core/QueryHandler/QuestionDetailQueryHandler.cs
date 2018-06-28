using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Core.QueryHandler
{
    //public class QuestionDetailQueryHandler: IQueryHandlerAsync<long,QuestionDetailDto>
    //{
    //    private readonly IQuestionRepository _questionRepository;
    //    private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;

    //    public QuestionDetailQueryHandler(IQuestionRepository questionRepository, IBlobProvider<QuestionAnswerContainer> blobProvider)
    //    {
    //        _questionRepository = questionRepository;
    //        _blobProvider = blobProvider;
    //    }

    //    public async Task<QuestionDetailDto> GetAsync(long query, CancellationToken token)
    //    {
    //        var dtoTask =  _questionRepository.GetQuestionDtoAsync(query, token);
    //        var filesTask = _blobProvider.FilesInDirectoryAsync($"question/{query}", token);
    //        await Task.WhenAll(dtoTask, filesTask).ConfigureAwait(false);
    //        var files = filesTask.Result;
    //        var dto = dtoTask.Result;

    //        if (dto == null)
    //        {
    //            return null;
    //        }
    //        //TODO should not be here
    //        var aggregateFiles = AggregateFiles(files);
    //        dto.Files = aggregateFiles[null];
    //        dto.Answers = dto.Answers.Select(s =>
    //        {
    //            s.Files = aggregateFiles[s.Id];
    //            return s;
    //        });

    //        return dto;
    //    }

    //    public static ILookup<Guid?, Uri> AggregateFiles(IEnumerable<Uri> files)
    //    {
    //        var aggregateFiles = files.ToLookup<Uri, Guid?>(v =>
    //        {
    //            if (v.Segments.Length == 5)
    //            {
    //                return null;
    //            }

    //            return Guid.Parse(v.Segments[5].Replace("/", string.Empty));
    //        });
    //        return aggregateFiles;
    //    }
    //}
}