using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace Zbang.Zbox.WebApi.Models
{
    public class ApiModel
    {
        IApiExplorer _explorer;

        public ApiModel(IApiExplorer explorer)
        {
            if (explorer == null)
            {
                throw new ArgumentNullException("explorer");
            }
            _explorer = explorer;
        }

        public ILookup<string, ApiDescription> GetApis()
        {
            return _explorer.ApiDescriptions.ToLookup(
                api => api.ActionDescriptor.ActionName);
        }
    }
}