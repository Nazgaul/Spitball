using Dapper;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Emails;
using Zbang.Zbox.ViewModel.SqlQueries;

namespace Zbang.Zbox.ReadServices
{
    public class ZboxReadServiceWorkerRole : IZboxReadServiceWorkerRole
    {
        public async Task<IEnumerable<UserDigestDto>> GetUsersByNotificationSettings(GetUserByNotificationQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<UserDigestDto>(Email.GetUserListByNotificationSettings,
                      new
                      {
                          Notification = query.NotificationSettings,
                          NotificationTime = query.MinutesPerNotificationSettings
                      });
            }

        }

        public async Task<IEnumerable<BoxDigestDto>> GetBoxesLastUpdates(GetBoxesLastUpdateQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                return await conn.QueryAsync<BoxDigestDto>(Email.GetBoxPossibleUpdateByUser,
                      new
                      {
                          Notification = query.MinutesPerNotificationSettings,
                          query.UserId
                      });
            }

        }

        public async Task<BoxUpdatesDigestDto> GetBoxLastUpdates(GetBoxLastUpdateQuery query)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync(string.Format("{0} {1} {2} {3} {4}",
                    Email.GetItemUpdateByBox,
                    Email.GetQuizUpdateByBox,
                    Email.GetQuizDiscussionUpdateByBox,
                    Email.GetQuestionUpdateByBox,
                    Email.GetAnswerUpdateByBox),
                     new
                      {
                          Notification = query.MinutesPerNotificationSettings,
                          query.BoxId
                      }))
                {
                    var retVal = new BoxUpdatesDigestDto
                    {
                        Items = await grid.ReadAsync<ItemDigestDto>(),
                        Quizzes = await grid.ReadAsync<QuizDigestDto>(),
                        QuizDiscussions = await grid.ReadAsync<QuizDiscussionDigestDto>(),
                        BoxComments = await grid.ReadAsync<QnADigestDto>(),
                        BoxReplies = await grid.ReadAsync<QnADigestDto>()
                    };
                    return retVal;
                }
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
                return retVal;
            }
        }

        public async Task<IEnumerable<dynamic>> GetMissingThumbnailBlobs(int index)
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
                // --thumbnailblobname in 
                //--(
                //--'filev4.jpg',
                //--'imagev4.jpg',
                //--'pdfv4.jpg',
                //--'powerv4.jpg',
                //--'wordv4.jpg',
                //--'excelv4.jpg'
                //--)

                return await conn.QueryAsync(@"select itemid, blobname from zbox.item 
                where  Discriminator = 'FILE'
                and isdeleted = 0
                and itemid > 70852
                order by itemid
                OFFSET @Offset ROWS
                FETCH NEXT @RowSize ROWS ONLY", new { Offset = index * 100, RowSize = 100 });

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

                //return await conn.QueryAsync<string>("select blobname from zbox.item where thumbnailUrl like '%v3.jpg' and name like '%.jpg' and isdeleted = 0");

            }
        }



        public async Task<IEnumerable<UniversitySearchDto>> GetUniversityDetail()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                var retVal = conn.Query<UniversitySearchDto>(LibraryChoose.GetUniversityDetail);
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


        public async Task<UniversityToUpdateSearchDto> GetUniversityDirtyUpdates()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                const string sql =
                    @"select top 500 id as Id,UniversityName as Name,LargeImage as Image,extra as Extra from zbox.University
                        where isdirty = 1 and isdeleted = 0;";
                const string sqlToDelete =
                    @"select top 500 id from zbox.University
                    where isdirty = 1 and isdeleted = 1";
                using (var grid = await conn.QueryMultipleAsync(sql + sqlToDelete))
                {
                    var retVal = new UniversityToUpdateSearchDto
                    {
                        UniversitiesToUpdate = await grid.ReadAsync<UniversitySearchDto>(),
                        UniversitiesToDelete = await grid.ReadAsync<long>()
                    };
                    return retVal;

                }

            }
        }

        public async Task<BoxToUpdateSearchDto> GetBoxDirtyUpdates()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {

                using (var grid = await conn.QueryMultipleAsync
                    (Search.GetBoxToUploadToSearch +
                    Search.GetBoxUsersToUploadToSearch +
                    Search.GetBoxToDeleteToSearch))
                {
                    var retVal = new BoxToUpdateSearchDto
                    {
                        BoxesToUpdate = await grid.ReadAsync<BoxSearchDto>()
                    };
                    var usersInBoxes = grid.Read<UsersInBoxSearchDto>().ToList();


                    foreach (var box in retVal.BoxesToUpdate)
                    {
                        box.UserIds = usersInBoxes.Where(w => w.BoxId == box.Id).Select(s => s.UserId);
                    }
                    retVal.BoxesToDelete = await grid.ReadAsync<long>();
                    return retVal;
                }

            }
        }

        public async Task<ItemToUpdateSearchDto> GetItemDirtyUpdatesAsync()
        {
            using (var conn = await DapperConnection.OpenConnectionAsync())
            {
                using (var grid = await conn.QueryMultipleAsync
                    (Search.GetItemsToUploadToSearch +
                    Search.GetItemUsersToUploadToSearch +
                    Search.GetItemToDeleteToSearch
                    ))
                {
                    var retVal = new ItemToUpdateSearchDto
                    {
                        ItemsToUpdate = await grid.ReadAsync<ItemSearchDto>()
                    };
                    var usersInItems = grid.Read<UsersInBoxSearchDto>().ToList();


                    foreach (var item in retVal.ItemsToUpdate)
                    {
                        item.UserIds = usersInItems.Where(w => w.BoxId == item.BoxId).Select(s => s.UserId);
                    }
                    retVal.ItemsToDelete = await grid.ReadAsync<long>();
                    return retVal;
                }
            }
        }

    }
}
