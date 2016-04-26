﻿using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
   public class PrivateBox : Box
    {
       protected PrivateBox()
       {
           
       }
       public PrivateBox(string boxName, User user, BoxPrivacySetting privacySetting, Guid newCommentId)
            : base(boxName,user,privacySetting,newCommentId)
        {
        }

       public override void CalculateMembers()
       {
           var count = MembersCount;
           base.CalculateMembers();
           if (count == MembersCount)
           {
               return;
           }
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
