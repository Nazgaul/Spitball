using Dapper;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Emails;
using Zbang.Zbox.ViewModel.SqlQueries;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadServiceWorkerRole : IZboxReadServiceWorkerRole
    {
        private readonly IBlobProvider m_BlobProvider;
        public ZboxReadServiceWorkerRole(IBlobProvider blobProvider)
        {
            m_BlobProvider = blobProvider;
        }
        public IEnumerable<UserDigestDto> GetUsersByNotificationSettings(GetUserByNotificationQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetUserListByNotificationSettings");
                dbQuery.SetEnum("Notification", query.NotificationSettings);
                dbQuery.SetInt32("NotificationTime", query.MinutesPerNotificationSettings);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<UserDigestDto>());
                return dbQuery.List<UserDigestDto>();
            }
        }

        public IEnumerable<BoxDigestDto> GetBoxesLastUpdates(GetBoxesLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxPossibleUpdateByUser");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("UserId", query.UserId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<BoxDigestDto>());
                return dbQuery.List<BoxDigestDto>().Select(s=> {
                    s.BoxPicture = string.IsNullOrEmpty(s.BoxPicture) ? "https://www.cloudents.com/images/emptyState/my_default3.png" : m_BlobProvider.GetThumbnailUrl(s.BoxPicture);
                    return s;
                });
            }
        }

        public IEnumerable<ItemDigestDto> GetItemsLastUpdates(GetItemsLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetItemUpdateByBox");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<ItemDigestDto>());
                return dbQuery.List<ItemDigestDto>().Select(s =>
                {
                    s.Picture = m_BlobProvider.GetThumbnailUrl(s.Picture);
                    return s;
                });
            }

        }
        public IEnumerable<QuizDigestDto> GetQuizLastUpdates(GetItemsLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetQuizUpdateByBox");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<QuizDigestDto>());
                return dbQuery.List<QuizDigestDto>();
            }
        }

        public IEnumerable<QuizDiscussionDigestDto> GetQuizDiscussion(GetCommentsLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetQuizDiscussionUpdateByBox");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<QuizDiscussionDigestDto>());
                return dbQuery.List<QuizDiscussionDigestDto>();
            }
        }

        public IEnumerable<QnADigestDto> GetQuestionsLastUpdates(GetCommentsLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetQuestionUpdateByBox");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<QnADigestDto>());
                return dbQuery.List<QnADigestDto>();
            }
        }
        public IEnumerable<QnADigestDto> GetAnswersLastUpdates(GetCommentsLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetAnswerUpdateByBox");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<QnADigestDto>());
                return dbQuery.List<QnADigestDto>();
            }
        }

        //public IEnumerable<MembersDigestDto> GetNewMembersLastUpdates(GetMembersLastUpdateQuery query)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetMembersUpdateByBox");
        //        dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
        //        dbQuery.SetInt64("BoxId", query.BoxId);
        //        dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<MembersDigestDto>());
        //        return dbQuery.List<MembersDigestDto>();
        //    }
        //}


        public BadItemDto GetFlagItemUserDetail(GetBadItemFlagQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBadItemUserDetail");
                dbQuery.SetInt64("UserId", query.UserId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<BadItemDto>());


                IQuery itemQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBadItemDetail");
                itemQuery.SetInt64("ItemId", query.ItemId);
                itemQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<BadItemDto>());

                var fdbQuery = dbQuery.FutureValue<BadItemDto>();
                var fitemQuery = itemQuery.FutureValue<BadItemDto>();

                var retVal = fdbQuery.Value;
                var itemVal = fitemQuery.Value;

                retVal.ItemName = itemVal.ItemName;
                retVal.BoxId = itemVal.BoxId;
                retVal.Uid = itemVal.Uid;
                return retVal;
            }
        }

        public async Task<IEnumerable<string>> GetMissingThumbnailBlobs()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
//                return await conn.QueryAsync<string>(@"select blobname from zbox.item
// where content is null and isdeleted = 0 and discriminator = 'FILE' 
//
// and SUBSTRING(blobname, 
//        LEN(blobname)-(CHARINDEX('.', reverse(blobname))-2), 8000) in 
//		('rtf', 'docx', 'doc', 'txt','pptx', 'potx', 'ppxs', 'ppsx', 'ppt', 'pot', 'pps','pdf','xls', 'xlsx', 'xlsm', 'xltx', 'ods', 'csv')
//
//
//");

//                return await conn.QueryAsync<string>(@"select blobname from zbox.item where thumbnailblobname in 
//(
//'filev4.jpg',
//'imagev4.jpg',
//'pdfv4.jpg',
//'powerv4.jpg',
//'wordv4.jpg',
//'excelv4.jpg'
//)
//and  isdeleted = 0");

//                return await conn.QueryAsync<string>(@"select blobname from zbox.item where 
//(blobname like '%.jpg'
//or blobname like '%.gif'
//or blobname like '%.png'
//or blobname like '%.jpeg'
//or blobname like '%.bmp')
//and isdeleted = 0 
//");

//                return await conn.QueryAsync<string>(@"select blobname from zbox.item
// where isdeleted = 0 and discriminator = 'FILE' 
//
// and SUBSTRING(blobname, 
//        LEN(blobname)-(CHARINDEX('.', reverse(blobname))-2), 8000) in 
//		('3gp', '3g2', '3gp2', 'asf', 'mts', 'm2ts', 'mod', 'dv', 'ts', 'vob', 'xesc', 'mp4', 'mpeg', 'mpg', 'm2v', 'ismv', 'wmv')");

                return await conn.QueryAsync<string>("select blobname from zbox.item where thumbnailUrl like '%v3.jpg' and name like '%.jpg' and isdeleted = 0");

            }
        }

       

        public async Task<IEnumerable<UniversityLuceneDto>> GetUniversityDetail()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = conn.Query<UniversityLuceneDto>(LibraryChoose.GetUniversityDetail);
                return retVal;
            }
        }

        public async Task<PartnersDto> GetPartnersEmail(long userid)
        {
            var retVal = new PartnersDto();
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(Email.Partners, new { userid }))
                {
                    retVal.LastWeekUsers = grid.Read<int>().First();
                    retVal.AllUsers = grid.Read<int>().First();

                    retVal.LastWeekCourses = grid.Read<int>().First();
                    retVal.AllCourses = grid.Read<int>().First();

                    retVal.LastWeekItems = grid.Read<int>().First();
                    retVal.AllItems = grid.Read<int>().First();

                    retVal.LastWeekQnA = grid.Read<int>().First();
                    retVal.AllQnA = grid.Read<int>().First();

                    retVal.Univeristies = grid.Read<University>();

                }

            }
            return retVal;

        }

    }
}
