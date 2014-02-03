using System;

namespace Zbang.Zbox.ViewModel.DTOs.ActivityDtos
{
    public class ActivityDto : BaseActivityDto
    {
        public ActivityDto(string userName, string userImg, long userUid, DateTime date, string boxName, string boxUid,
            long itemUid, string itemName)
            : base(userName, userImg, userUid, date, boxName, boxUid,
                itemUid, itemName)
        {

        }
    }
}
