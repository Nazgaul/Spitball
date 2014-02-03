using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using ZboxDto = Zbang.Zbox.ViewModel.DTOs;
using ZboxQuery = Zbang.Zbox.ViewModel.Queries;
using ExtensionTransformers = Zbang.Zbox.Infrastructure.Data.Transformers;
using Zbang.Zbox.ApiViewModel.DTOs;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ApiViewModel.Queries;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Cache;


namespace Zbang.Zbox.ReadServices
{
    public partial class ZboxApiReadService : BaseReadService, IZboxApiReadService
    {
        public ZboxApiReadService(IHttpContextCacheWrapper contextCacheWrapper)
            : base(contextCacheWrapper)
        {

        }
        public IList<ApiBoxDto> GetBoxes(GetBoxesQueryBase query)
        {
            using (UnitOfWork.Start())
            {

                IQuery queryGetBoxDtosByStorageId = UnitOfWork.CurrentSession.GetNamedQuery(query.QueryName);//"APIGetBoxes");
                queryGetBoxDtosByStorageId.SetProperties(query);
                //queryGetBoxDtosByStorageId.SetInt64("UserId", query.UserId);
                IList<ApiBoxDto> boxes = queryGetBoxDtosByStorageId.List<ApiBoxDto>();
                return boxes;
            }
        }

        //public IEnumerable<UserDto> GetUserFriends(ZboxQuery.GetUserFriendsQuery query)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        IQuery queryGetUserFriends = UnitOfWork.CurrentSession.GetNamedQuery("ApiGetUserFriends");
        //        queryGetUserFriends.SetInt64("UserId", query.UserId);
        //        queryGetUserFriends.SetResultTransformer(Transformers.AliasToBeanConstructor(typeof(UserDto).GetConstructors()[0]));
        //        return queryGetUserFriends.List<UserDto>();
        //    }
        //}


        /// <summary>
        /// Get the users connected to this box Invites+subscribers+owner
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<ZboxDto.UserPublicSettingDto> GetBoxUsers(ZboxQuery.GetBoxUsersQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery queryBoxUsers = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxUsers");
                queryBoxUsers.SetParameter("BoxId", query.BoxId);
                queryBoxUsers.SetResultTransformer(Transformers.AliasToBean<ZboxDto.UserPublicSettingDto>());
                return queryBoxUsers.List<ZboxDto.UserPublicSettingDto>();
            }
        }


        #region newDesignofApi

        public IList<ItemDto> GetBoxItems(ZboxQuery.GetBoxItemsPagedQuery query)
        {
            using (UnitOfWork.Start())
            {

                var queryBoxItem = UnitOfWork.CurrentSession.GetNamedQuery("APIGetBoxItemDtosByBoxId");

                queryBoxItem.SetInt64("BoxId", query.BoxId);
                queryBoxItem.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClassesCtorTransformer(typeof(FileDto), typeof(LinkDto)));

                //queryBoxItem.SetFirstResult(query.PageNumber * query.MaxResult);
                //queryBoxItem.SetMaxResults(query.MaxResult);


                var items = queryBoxItem.Future<ItemDto>();
                CheckIfUserAllowedToSee(query.BoxId, query.UserId);
                return items.ToList();
            }
        }

        public ItemDto GetBoxItem(ZboxQuery.GetItemQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery queryBoxItem = UnitOfWork.CurrentSession.GetNamedQuery("APIGetBoxItemDtoByItemId");

                queryBoxItem.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClassesCtorTransformer(typeof(FileDto), typeof(LinkDto)));

                queryBoxItem.SetInt64("ItemId", query.ItemId);

                var item = queryBoxItem.FutureValue<ItemDto>();
                CheckIfUserAllowedToSee(query.BoxId, query.UserId);

                if (item.Value == null)
                {
                    throw new ItemNotFoundException();
                }
                return item.Value;
            }
        }

        public IEnumerable<ApiCommentDto> GetBoxComments(ZboxQuery.GetBoxCommentsQuery query)
        {
            using (UnitOfWork.Start())
            {
                IQuery getBoxCommentsQuery = UnitOfWork.CurrentSession.GetNamedQuery("APIGetBoxComments");
                getBoxCommentsQuery.SetParameter("boxid", query.BoxId);
                var comments = getBoxCommentsQuery.Future<ApiCommentDto>();

                CheckIfUserAllowedToSee(query.BoxId, query.UserId);

                return comments;
            }
        }

        //public IEnumerable<ApiCommentDto> GetItemComments(ZboxQuery.GetItemCommentsQuery query)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        IQuery getiCmntsQuery = UnitOfWork.CurrentSession.GetNamedQuery("APIGetItemComments");

        //        getiCmntsQuery.SetParameter("boxid", query.BoxId);
        //        getiCmntsQuery.SetParameter("itemid", query.ItemId);
        //        return getiCmntsQuery.List<ApiCommentDto>();
        //    }
        //}

        /// <summary>
        /// Paged query - doesnt user check user because the data is based on the user id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<ApiTextDto> GetUserWall(GetWallQuery query)
        {
            using (UnitOfWork.Start())
            {
                var getRecentActivityQuery = UnitOfWork.CurrentSession.GetNamedQuery("APIGetRecenetAcitivityAll");
                getRecentActivityQuery.SetInt64("UserId", query.UserId);

                //getRecentActivityQuery.SetFirstResult(query.PageNumber * query.MaxResult);
                //getRecentActivityQuery.SetMaxResults(query.MaxResult);


                return getRecentActivityQuery.List<ApiTextDto>();
            }
        }


        /// <summary>
        /// Get box data - also contain box comment and box picture
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        //public ZboxDto.BoxWithDetailDto GetBox(GetBoxQuery query)
        //{
        //    using (UnitOfWork.Start())
        //    {
        //        IQuery boxQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxWithDetailDtoById");
        //        boxQuery.SetParameter("Id", query.BoxId);
        //        //boxQuery.SetParameter("UserId", query.UserId);
        //        boxQuery.SetResultTransformer(Transformers.AliasToBean<ZboxDto.BoxWithDetailDto>());

        //        IQuery boxCommentsQuery = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxComments");
        //        boxCommentsQuery.SetParameter("boxid", query.BoxId);

        //        IQuery getBoxImage = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxImageById");
        //        getBoxImage.SetParameter("Id", query.BoxId);
        //        getBoxImage.SetMaxResults(1);

        //        IQuery queryBoxItem = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxItemDtosByBoxId");
        //        queryBoxItem.SetInt64("BoxId", query.BoxId);
        //        queryBoxItem.SetResultTransformer(ExtensionTransformers.Transformers.AliasToDerivedClasses(typeof(FileDto), typeof(LinkDto)));
        //        queryBoxItem.SetFirstResult(query.PageNumber);
        //        queryBoxItem.SetMaxResults(query.MaxResult);


        //        IQuery queryCountBoxItem = UnitOfWork.CurrentSession.GetNamedQuery("GetBoxItemCountByBoxId");
        //        queryCountBoxItem.SetInt64("BoxId", query.BoxId);

        //        //TODO: we already get this on GetUserPermission - we have duplicate query 
        //        //IQuery queryGetUserPermission = UnitOfWork.CurrentSession.GetNamedQuery("GetUserRelationToBox");
        //        //queryGetUserPermission.SetInt64("UserId", query.UserId);
        //        //queryGetUserPermission.SetInt64("BoxId", query.BoxId);
        //        //queryGetUserPermission.SetResultTransformer(Transformers.AliasToBean<UserType>());




        //        var fbox = boxQuery.FutureValue<ZboxDto.BoxWithDetailDto>();
        //        var fimage = getBoxImage.FutureValue<string>();

        //        var fcomments = boxCommentsQuery.Future<ZboxDto.CommentDto>();

        //        var fitems = queryBoxItem.Future<ZboxDto.ItemDto>();
        //        var fitemCount = queryCountBoxItem.FutureValue<int>();
        //        //var fuserType = queryGetUserPermission.FutureValue<UserRelationshipType>();


        //        var userType = CheckIfUserAllowedToSee(query.BoxId, query.UserId);
        //        ZboxDto.BoxWithDetailDto box = fbox.Value;

        //        if (box == null)
        //            throw new BoxDoesntExistException();
        //        box.Comments = fcomments.ToList();
        //        box.Image = fimage.Value; // for head link_Src
        //        box.Items.Count = fitemCount.Value;
        //        box.UserType = userType;
        //        box.Items.Dto = fitems.ToList();
        //        return box;
        //    }
        //}

        #endregion
    }
}
