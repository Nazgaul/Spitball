using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Query
{
    public class QuestionDataByIdQuery : IQuery<QuestionDetailDto>

    {
        public QuestionDataByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}