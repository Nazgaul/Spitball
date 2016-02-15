using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
   public class PrivateBox : Box
    {
       protected PrivateBox()
       {
           
       }
       public PrivateBox(string boxName, User user, BoxPrivacySettings privacySettings, Guid newCommentId)
            : base(boxName,user,privacySettings,newCommentId)
        {
        }

       public override void CalculateMembers()
       {
           base.CalculateMembers();
           foreach (var item in Items)
           {
               item.IsDirty = true;
           }
           foreach (var quiz in Quizzes)
           {
               quiz.IsDirty = true;
           }
       }
    }
}
