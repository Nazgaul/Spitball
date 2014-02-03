using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.ViewModel.DTOs.BoxDtos;

namespace Zbang.Zbox.ViewModel.Queries.Box
{
    public class GetAcademicBoxUploadDataQuery : GetBoxUploadDataQuery
    {
        public GetAcademicBoxUploadDataQuery(long boxId, long userId)
            : base(boxId, userId)
        {
            QueryName = "GetAcademicBoxUploadData";
            ReturnValue = typeof(AcademicBoxUploadData);
        }
    }
}
