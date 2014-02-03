
using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ApiViewModel.DTOs
{
    [DataContract]
    class ApiActionDto : ApiTextDto
    {

        protected ActionType ActionType { get; set; }

        [DataMember]
        public string ActionTypeasString
        {
            get { return ActionType.ToString();  }
            protected set { }
        }
        //[DataMember]
        //public override string CommentType
        //{
        //    get { return "Action"; }
        //}
    }
}
