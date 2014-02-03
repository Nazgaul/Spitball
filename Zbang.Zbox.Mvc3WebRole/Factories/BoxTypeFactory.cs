using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Queries.Autocomplete;
using Zbang.Zbox.ViewModel.Queries.Box;

namespace Zbang.Zbox.Mvc3WebRole.Factories
{
    public class BoxTypeFactory
    {
        public GetBoxByNameQuery GetBoxByNameQuery(BoxType? type, string term, long userId)
        {
            switch (type)
            {
                case BoxType.Box:
                    return new GetBoxByNameQuery(term, userId);
                case BoxType.Academic:
                    return new GetAcademicBoxByNameQuery(term, userId);
                default:
                    return new GetBoxByNameQuery(term, userId);

            }
        }

        public GetBoxUploadDataQuery GetUploadBoxDataQuery(BoxType? type, long boxid, long userId)
        {
            switch (type)
            {
                case BoxType.Box:
                    return new GetBoxUploadDataQuery(boxid, userId);
                case BoxType.Academic:
                    return new GetAcademicBoxUploadDataQuery(boxid, userId);
                default:
                    return new GetBoxUploadDataQuery(boxid, userId);

            }
        }
    }
}