using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
