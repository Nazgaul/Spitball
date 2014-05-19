using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
   public  class UserInviteDto
    {
       public string UserImage { get; set; }
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
                   var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                   m_BoxPicture = blobProvider.GetThumbnailUrl(value);
               }
           }
       }
    }
}
