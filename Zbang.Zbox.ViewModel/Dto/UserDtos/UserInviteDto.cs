﻿using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
   public  class UserInviteDto
    {
       public string UserImage { get; set; }
       //TODO: fix that to user Name
       public string Username { get; set; }
       public long Userid { get; set; }
       public MessageType InviteType { get; set; }
       public string BoxName { get; set; }
       public long Boxid { get; set; }
       public bool Status { get; set; }


       private string m_BoxPicture;
       public string BoxPicture
       {
           get
           {
               return m_BoxPicture;
           }
           set
           {
               if (!string.IsNullOrEmpty(value))
               {
                   //var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                   m_BoxPicture = value;// blobProvider.GetThumbnailUrl(value);
               }
           }
       }
    }
}
