﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using NHibernate;
using Cloudents.Query.Stuff;
using Cloudents.Core.DTOs.Documents;

namespace Cloudents.Query.Documents
{
    public class SimilarDocumentsQuery : IQuery<IEnumerable<DocumentFeedDto>>
    {
        public SimilarDocumentsQuery(long documentId)
        {
            DocumentId = documentId;
        }
        public long DocumentId { get; }
    }

    internal sealed class SimilarDocumentsQueryHandler : IQueryHandler<SimilarDocumentsQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;
        private readonly IUrlBuilder _urlBuilder;

        public SimilarDocumentsQueryHandler(QuerySession session, IUrlBuilder urlBuilder)
        {
            _urlBuilder = urlBuilder;
            _session = session.StatelessSession;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(SimilarDocumentsQuery query, CancellationToken token)
        {
            const string sql = @"with cte as
                                (
                                select  CourseName, u.SbCountry
                                from sb.Document d 
                                join sb.[user] u
	                                on d.UserId = u.Id
                                where d.Id = :Id
                                )

                                select top 10 d.Id, d.UpdateTime as DateTime, 
                                d.CourseName as Course,
d.Name as Title,  
                                coalesce(d.Description, d.MetaContent) as Snippet,
d.Views, 
d.Downloads,
d.Price, 
                                d.DocumentType, d.Duration,  d.PriceType,
                                (
	                                select count(1)
	                                from sb.[Transaction] t 
	                                where t.TransactionType='Document' and 
	                                t.DocumentId=d.Id
	                                and t.[Action]='SoldDocument'
                                ) as Purchased, 
                                d.VoteCount as [Vote.Votes],
                                u.Id as [User.Id], u.Name as [User.Name], 
                                u.ImageName as [User.Image]
                                from sb.[Document] d 
                                inner join sb.[User] u on d.UserId=u.Id ,
                                cte
                                where d.CourseName = cte.CourseName and u.SbCountry = cte.SbCountry
                                and d.Id != :Id and d.[State]='ok'
                                order by d.DocumentType desc, d.UpdateTime desc;
                                ";

            var res = await _session.CreateSQLQuery(sql)
                .SetInt64("Id", query.DocumentId)
                .SetResultTransformer(new DeepTransformer<DocumentFeedDto>('.', new SbAliasToBeanResultTransformer<DocumentFeedDto>()))
                .ListAsync<DocumentFeedDto>(token);

            return res.Select(s =>
            {
                s.User.Image = _urlBuilder.BuildUserImageEndpoint(s.User.Id, s.User.Image);
                s.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(s.Id);
                return s;
            });
        }
    }
}
