using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class QuestionDetailQuery : IQuery<QuestionDetailDto>
    {
        public QuestionDetailQuery(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }
}