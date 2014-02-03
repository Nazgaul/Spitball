using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.ViewModel.DTOs.ActivityDtos
{
    public class BaseActivityDto
    {
        public BaseActivityDto(string userName, string userImg, long userUid, DateTime date, string boxName, string boxUid, 
            long? itemUid,  string itemName)
        {
            UserName = userName;
            UserUid = userUid;
            UserImg = userImg;
            Date = date;
            BoxName = boxName;
            BoxUid = boxUid;
            ItemName = itemName;
            ItemUid = itemUid;
        }
        public string UserName { get; private set; }
        public string UserImg { get; private set; }
        public long UserUid { get; private set; }
        public DateTime Date { get; private set; }
        public string BoxName { get; private set; }
        public string BoxUid { get; private set; }
        public long? ItemUid { get; private set; }
        public string ItemName { get; private set; }
    }
}
