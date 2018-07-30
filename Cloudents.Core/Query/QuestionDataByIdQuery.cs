using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
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