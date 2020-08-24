using System;

namespace Cloudents.Web.Binders
{
    [Flags]
    public enum ProfileServiceQuery
    {
        None,
        Country = 2,
        //  Course = 4,
        Subscribers = 8
    }
}