using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class SeoQuizPage : IReadRepositoryAsync<QuizSeoDto, long>
    {
        private readonly DapperRepository _repository;

        public SeoQuizPage(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<QuizSeoDto> GetAsync(long query, CancellationToken token)
        {
            return _repository.WithConnectionAsync(c => c.QueryFirstOrDefaultAsync<QuizSeoDto>(
                @"select u.country as Country
,b.boxName as BoxName,
q.Name,
		(select top(1) qq.Text from zbox.QuizQuestion qq where QuizId = q.id) as FirstQuestion,
		l.name as departmentName
        from 
		zbox.quiz q
		join zbox.box b on q.boxId= b.boxId
		left join zbox.library l on b.libraryId = l.libraryId
		where q.id = @QuizId
        and q.publish = 1
        and q.isDeleted = 0;", new { QuizId = query }), token);
        }
    }
}
