using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using System.Globalization;

namespace Cloudents.Query.Email
{
    public class GetEmailByEventQuery : IQuery<IEnumerable<EmailObjectDto>>
    {
        public GetEmailByEventQuery(string @event)
        {
            Event = @event;
        }
        public string Event { get; set; }
    }

    internal sealed class GetEmailByEventQueryHandler :
        IQueryHandler<GetEmailByEventQuery, IEnumerable<EmailObjectDto>>
    {
        private readonly IConfigurationKeys _provider;

        public GetEmailByEventQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }

        public async Task<IEnumerable<EmailObjectDto>> GetAsync(GetEmailByEventQuery query, CancellationToken token)
        {
            const string sql = @"SELECT JSON_VALUE(template, '$.socialShare') as SocialShare,
	                            JSON_VALUE(template, '$.eventName') as Event,
	                            JSON_VALUE(template, '$.subject') as Subject,
	                            JSON_VALUE(template, '$.cultureInfo') as CultureInfo,
	                            JSON_VALUE(template, '$.blocks[0].title') as Title0,
	                            JSON_VALUE(template, '$.blocks[0].subtitle') as Subtitle0,
	                            JSON_VALUE(template, '$.blocks[0].body') as Body0,
	                            JSON_VALUE(template, '$.blocks[0].cta') as Cta0,
	                            JSON_VALUE(template, '$.blocks[0].minorTitle') as MinorTitle0,
	                            JSON_VALUE(template, '$.blocks[1].title') as Title1,
	                            JSON_VALUE(template, '$.blocks[1].subtitle') as Subtitle1,
	                            JSON_VALUE(template, '$.blocks[1].body') as Body1,
	                            JSON_VALUE(template, '$.blocks[1].cta') as Cta1,
	                            JSON_VALUE(template, '$.blocks[1].minorTitle') as MinorTitle1
                            FROM sb.Email
                            WHERE JSON_VALUE(template, '$.eventName') = @e";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
              
                var flatRes = await connection.QueryAsync<FlatEmailObject>(sql,
                    new
                    {
                        e = query.Event,
                    });

                List<EmailObjectDto> result = new List<EmailObjectDto>();
                foreach (var res in flatRes)
                {
                    var t = new EmailObjectDto()
                    {
                        SocialShare = Boolean.Parse(res.SocialShare),
                        Event = res.Event,
                        Subject = res.Subject,
                        CultureInfo = new CultureInfo(res.CultureInfo),
                        Blocks = new List<EmailBlockDto>()
                    };
                    t.Blocks.Add(new EmailBlockDto()
                    {
                        Title = res.Title0,
                        Subtitle = res.Subtitle0,
                        Body = res.Body0,
                        Cta = res.Cta0,
                        MinorTitle = res.MinorTitle0
                    });
                    if (!string.IsNullOrEmpty(res.Body1))
                    {
                        t.Blocks.Add(new EmailBlockDto()
                        {
                            Title = res.Title1,
                            Subtitle = res.Subtitle1,
                            Body = res.Body1,
                            Cta = res.Cta1,
                            MinorTitle = res.MinorTitle1
                        });
                    }
                    result.Add(t);
                }
                return result;
            }
        }

        public class FlatEmailObject
        {
            public string SocialShare { get; set; }
            public string Event { get; set; }
            public string Subject { get; set; }
            public string CultureInfo { get; set; }
            public string Title0 { get; set; }
            public string Subtitle0 { get; set; }
            public string Body0 { get; set; }
            public string Cta0 { get; set; }
            public string MinorTitle0 { get; set; }
            public string Title1 { get; set; }
            public string Subtitle1 { get; set; }
            public string Body1 { get; set; }
            public string Cta1 { get; set; }
            public string MinorTitle1 { get; set; }
        }
    }
    
}