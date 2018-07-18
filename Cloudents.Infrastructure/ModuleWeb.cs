using System.Collections.Generic;
using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.Data;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Core.Enum.System.Web)]
    [UsedImplicitly]
    public class ModuleWeb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SeoDocumentRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Flashcard).WithParameter("query", SeoDbQuery.Flashcard);
            builder.RegisterType<SeoDocumentRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Item).WithParameter("query", SeoDbQuery.Document);
        }
    }
}
