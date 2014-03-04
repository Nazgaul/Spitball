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

        public IEnumerable<CommentDigestDto> GetQuestionsLastUpdates(GetCommentsLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetCommentUpdateByBox");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<CommentDigestDto>());
                return dbQuery.List<CommentDigestDto>();
            }
        }
        public IEnumerable<CommentDigestDto> GetAnswersLastUpdates(GetCommentsLastUpdateQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery dbQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetAnswerUpdateByBox");
                dbQuery.SetInt32("Notification", query.MinutesPerNotificationSettings);
                dbQuery.SetInt64("BoxId", query.BoxId);
                dbQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<CommentDigestDto>());
                return dbQuery.List<CommentDigestDto>();
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
              return await conn.QueryAsync<string>(@"select blobname from zbox.item where thumbnailblobname in 
('filev2.jpg',
'filev3.jpg',
'filev4.jpg',
'imagev1.jpg',
'imagev4.jpg',
'linkv2.jpg',
'musicv1.jpg',
'pdfv1.jpg',
'pdfv4.jpg',
'powerpointv1.jpg',
'powerv4.jpg',
'soundv4.jpg',
'videov1.jpg',
'videov4.jpg',
'wordv1.jpg',
'wordv4.jpg',
'excelv1.jpg',
'excelv4.jpg')
and  isdeleted = 0");                
            }
        }
    }
}
