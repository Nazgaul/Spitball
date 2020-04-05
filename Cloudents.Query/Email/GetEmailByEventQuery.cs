using Cloudents.Core.DTOs.Email;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class GetEmailByEventQuery : IQuery<IEnumerable<EmailObjectDto>>
    {
        public GetEmailByEventQuery(string @event)
        {
            Event = @event;
        }
        private string Event { get; }


        internal sealed class GetEmailByEventQueryHandler :
            IQueryHandler<GetEmailByEventQuery, IEnumerable<EmailObjectDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public GetEmailByEventQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<EmailObjectDto>> GetAsync(GetEmailByEventQuery query, CancellationToken token)
            {
                const string sql = @"SELECT JSON_VALUE(template, '$.socialShare') as SocialShare,
	                            JSON_VALUE(template, '$.eventName') as Event,
	                            JSON_VALUE(template, '$.subject') as Subject,
	                            JSON_VALUE(template, '$.cultureInfo') as CultureInfo,
	                            JSON_VALUE(template, '$.blocks[0].title') as Title,
	                            JSON_VALUE(template, '$.blocks[0].subtitle') as Subtitle,
	                            JSON_VALUE(template, '$.blocks[0].body') as Body,
	                            JSON_VALUE(template, '$.blocks[0].cta') as Cta,
	                            JSON_VALUE(template, '$.blocks[0].minorTitle') as MinorTitle,
	                            JSON_VALUE(template, '$.blocks[1].title') as Title,
	                            JSON_VALUE(template, '$.blocks[1].subtitle') as Subtitle,
	                            JSON_VALUE(template, '$.blocks[1].body') as Body,
	                            JSON_VALUE(template, '$.blocks[1].cta') as Cta,
	                            JSON_VALUE(template, '$.blocks[1].minorTitle') as MinorTitle
                            FROM sb.Email
                            WHERE JSON_VALUE(template, '$.eventName') = @e";
                using var conn = _dapperRepository.OpenConnection();
                var flatRes = await conn.QueryAsync<EmailObjectDto, EmailBlockDto, EmailBlockDto, EmailObjectDto>(sql,
                    (emailDto, blockDto0, blockDto1) =>
                    {
                        emailDto.Blocks.Add(blockDto0);
                        if (blockDto1 != null)
                        {
                            emailDto.Blocks.Add(blockDto1);
                        }
                        return emailDto;
                    }, new
                    {
                        e = query.Event,
                    }, splitOn: "SocialShare,Title,Title");
                return flatRes;
            }
        }
    }

}