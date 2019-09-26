using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public sealed class DocumentMap : ClassMapping<Document>
    {
        public DocumentMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.HighLow, g => g.Params(
                new
                {
                    table = nameof(HiLoGenerator),
                    column = nameof(HiLoGenerator.NextHi),
                    max_lo = 10,
                    where = $"{nameof(HiLoGenerator.TableName)}='{nameof(Document)}'"
                })));
            //Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10",
            //    $"{nameof(HiLoGenerator.TableName)}='{nameof(Document)}'");
            Property(x => x.Name, c => {
                c.NotNullable(true);
                c.Length(150);
            });
            //Map(x => x.Name).Length(150).Not.Nullable();
            ManyToOne(x => x.University, c => {
                c.NotNullable(true);
                c.Column("UniversityId");
                c.ForeignKey("Document_University");
            });
            //References(x => x.University).Not.Nullable().Column("UniversityId").ForeignKey("Document_University");

            Property(x => x.Type, c => c.NotNullable(true));
            //Map(x => x.Type).Not.Nullable();

            Set(x => x.Tags, c =>
            {
                c.Table("DocumentsTags");
                c.Key(k => {
                    k.Column("DocumentId");
                    k.ForeignKey("Document_Tags");
                    k.ForeignKey("Tags_Documents");
                    });
            }, m => m.ManyToMany(p => p.Column("TagId")));
            //HasManyToMany(x => x.Tags)
            //    .ParentKeyColumn("DocumentId")
            //    .ChildKeyColumn("TagId")
            //    .ForeignKeyConstraintNames("Document_Tags", "Tags_Documents")
            //    .Table("DocumentsTags").AsSet();

            Component(x => x.TimeStamp);
            //Component(x => x.TimeStamp);

            ManyToOne(x => x.Course, c => {
                c.Column("CourseName");
                c.NotNullable(true);
                c.ForeignKey("Document_course");
            });
            //References(x => x.Course).Column("CourseName").Not.Nullable().ForeignKey("Document_course");

            ManyToOne(x => x.User, c => {
                c.Column("UserId");
                c.NotNullable(true);
                c.ForeignKey("Document_User");
            });
            //References(x => x.User).Column("UserId").Not.Nullable().ForeignKey("Document_User");
            Property(x => x.Views, c => c.NotNullable(true));
            //Map(x => x.Views).Not.Nullable();
            Property(x => x.Downloads, c => c.NotNullable(true));
            //Map(x => x.Downloads).Not.Nullable();
            Property(x => x.Professor, c => c.NotNullable(false));
            //Map(x => x.Professor).Nullable();
            Property(x => x.PageCount, c => c.NotNullable(false));
            //Map(x => x.PageCount).Nullable();
            ////Map(x => x.Purchased).Not.Nullable();
            Property(x => x.MetaContent, c => c.NotNullable(false));
            //Map(x => x.MetaContent).Nullable();

            Property(x => x.Price, c => {
                c.NotNullable(true);
                c.Column(cl => cl.SqlType("smallmoney"));
            });
            //Map(x => x.Price).Not.Nullable().CustomSqlType("smallmoney");
            Bag<Transaction>("_transactions", c => {
                c.Key(k => k.Column("DocumentId"));
                c.Lazy(CollectionLazy.Lazy);
                c.Inverse(true);
            }, a => a.OneToMany());
            ////DO NOT PUT ANY CASCADE WE HANDLE THIS ON CODE - TAKE A LOOK AT ADMIN COMMAND AND REGULAR COMMAND
            //HasMany(x => x.Transactions)
            //    .KeyColumn("DocumentId")
            //    //.Cascade.()
            //    .Access.CamelCaseField(Prefix.Underscore)
            //    .LazyLoad()
            //    .Inverse();
            Property(x => x.OldId, c => c.NotNullable(false));
            //Map(x => x.OldId).Nullable();
            Bag<Vote>("_votes", c => {
                c.Key(k => k.Column("DocumentId"));
                c.Inverse(true);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
            }, a => a.OneToMany());
            //HasMany(x => x.Votes)
            //    .Access.CamelCaseField(Prefix.Underscore)
            //    .KeyColumns.Add("DocumentId")
            //    .Inverse().Cascade.AllDeleteOrphan();
            Property(m => m.VoteCount);
            //Map(m => m.VoteCount);

            Component(x => x.Status);
            //Component(x => x.Status);
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //SchemaAction.Validate();
        }
    }
}