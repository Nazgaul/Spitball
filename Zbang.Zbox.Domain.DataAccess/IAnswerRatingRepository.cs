using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IAnswerRatingRepository: IRepository<AnswerRating> 
    {
        AnswerRating GetAnswerRating(long userId, Guid answerId);
    }
}
