using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.ViewModel.DTOs.BoxDtos;

namespace Zbang.Zbox.ViewModel.Queries.Box
{
    public class GetBoxUploadDataQuery : GetBoxQuery
    {
        public GetBoxUploadDataQuery(long boxId, long userId)
            : base(boxId, userId)
        {
            QueryName = "GetBoxUploadData";
            ReturnValue = typeof(BoxUploadData);
        }
        public string QueryName { get; protected set; }

        public Type ReturnValue { get; set; }
    }
}
