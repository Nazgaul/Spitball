using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class UserDownloadDocumentMap : ClassMap<UserDownloadDocument>
    {
        public UserDownloadDocumentMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Column("UserId").Not.Nullable().UniqueKey("c_user_download");
            References(x => x.Document).Column("DOcumentId").Not.Nullable().UniqueKey("c_user_download");
            Map(x => x.Created).Insert().Not.Update();
            Table("DocumentDownload");
        }
    }
}
