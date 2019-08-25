using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public sealed class GoogleTokensMap : ClassMap<GoogleTokens>
    {
        public GoogleTokensMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Value).Length(8000);

        }
    }
}