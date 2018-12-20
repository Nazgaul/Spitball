using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
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