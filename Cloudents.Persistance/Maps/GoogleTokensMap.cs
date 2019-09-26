using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
//using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public sealed class GoogleTokensMap : ClassMapping<GoogleTokens>
    {
        public GoogleTokensMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.Assigned));
            //Id(x => x.Id).GeneratedBy.Assigned();
            Property(x => x.Value, c => c.Length(8000));
            //Map(x => x.Value).Length(8000);

        }
    }
}