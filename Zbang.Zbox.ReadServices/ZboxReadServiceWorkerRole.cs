using Dapper;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.ViewModel.DTOs.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadServiceWorkerRole : IZboxReadServiceWorkerRole
    {
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
                return dbQuery.List<BoxDigestDto>();
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
                return dbQuery.List<ItemDigestDto>();
            }
        
        }
        public IEnumerable<QuizDigestDto> GetQuizLastpdates(GetItemsLastUpdateQuery query)
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

        public IEnumerable<MembersDigestDto> GetNewMembersLastUpdates(GetMembersLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetMembersUpdateByBox");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<MembersDigestDto>());
                return dbQuery.List<MembersDigestDto>();
            }
        }


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
                retVal.BoxUid = itemVal.BoxUid;
                retVal.Uid = itemVal.Uid;
                return retVal;
            }
        }

        public async Task<IEnumerable<string>> GetMissingThumbnailBlobs()
        {
            using (var conn = await DapperConnection.OpenConnection())
            {
                return await conn.QueryAsync<string>(@"select blobname from zbox.item
 where content is null and isdeleted = 0 and discriminator = 'FILE' 

 and SUBSTRING(blobname, 
        LEN(blobname)-(CHARINDEX('.', reverse(blobname))-2), 8000) in 
		('rtf', 'docx', 'doc', 'txt','pptx', 'potx', 'ppxs', 'ppsx', 'ppt', 'pot', 'pps','pdf','xls', 'xlsx', 'xlsm', 'xltx', 'ods', 'csv')


");
            }
        }

        public async Task<PartnersDto> GetPartnersEmail(long userid)
        {
            PartnersDto retVal = new PartnersDto();
            using (var conn = await DapperConnection.OpenConnection())
            {
                using (var grid = conn.QueryMultiple(Zbang.Zbox.ViewModel.SqlQueries.Email.partners, new { userid = userid }))
                {
                    retVal.LastWeekUsers = grid.Read<int>().First();
                    retVal.AllUsers = grid.Read<int>().First();

                    retVal.LastWeekCourses = grid.Read<int>().First();
                    retVal.AllCourses = grid.Read<int>().First();

                    retVal.LastWeekItems = grid.Read<int>().First();
                    retVal.AllItems = grid.Read<int>().First();

                    retVal.LastWeekQnA = grid.Read<int>().First();
                    retVal.AllQnA= grid.Read<int>().First();

                    retVal.Univeristies = grid.Read<Univeristy>();

                }

            }
            return retVal;

        }

    }
}
