using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.DTOs
{
    [Serializable]
    [DataContract]
    class ActionDto : TextDto
    {

        protected ActionType ActionType { get; set; }

        [DataMember]
        public string ActionTypeasString
        {
            get { return ActionType.ToString(); }
        }
        [DataMember]
        public override string CommentType
        {
            get { return "Action"; }
        }
    }
}
