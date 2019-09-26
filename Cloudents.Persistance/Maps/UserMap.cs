using System.Globalization;
using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;

namespace Cloudents.Persistence.Maps
{
    public class BaseUserMap : ClassMapping<BaseUser>
    {
        public BaseUserMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.HighLow, g => g.Params(
                new
                {
                    table = nameof(HiLoGenerator),
                    column = nameof(HiLoGenerator.NextHi),
                    max_lo = 10,
                    where = $"{nameof(HiLoGenerator.TableName)}='User'"
                })));
            //Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10", $"{nameof(HiLoGenerator.TableName)}='User'");
            Property(x => x.Email, c => {
                c.Unique(true);
            });
            //Map(e => e.Email)/*.Not.Nullable()*/.Unique();
            Property(x => x.Name, c => c.NotNullable(true));
            //Map(e => e.Name).Not.Nullable();
            Property(x => x.EmailConfirmed);
            //Map(e => e.EmailConfirmed);

            ////Map(e => e.NormalizedName);
            ////Map(e => e.NormalizedEmail);
            
            Property(x => x.SecurityStamp);
            //Map(e => e.SecurityStamp);
            Property(x => x.Image, c=> {
                c.Length(5000);
                c.NotNullable(false);
            });
            //Map(e => e.Image).Length(5000).Nullable();
            Property(x => x.ImageName, c => {
                c.Length(5000);
                c.NotNullable(false);
            });
            //Map(e => e.ImageName).Length(5000).Nullable();
            Property(x => x.AuthenticatorKey);
            //Map(e => e.AuthenticatorKey);
            //// Map(e => e.Culture);


            Property(x => x.Country, c => {
                c.NotNullable(false);
                c.Length(2);
            });
            //Map(e => e.Country).Nullable().Length(2);

            Property(x => x.Created, c => {
                c.Insert(true);
                c.Update(false);
            });
            //Map(e => e.Created).Insert().Not.Update();
            Property(x => x.Fictive, c => {
                c.Access(Accessor.ReadOnly);
                c.Insert(false);
                c.Update(false);
            });
            //Map(e => e.Fictive).ReadOnly();

            Property(x => x.OldUser, c => c.NotNullable(false));
            //Map(e => e.OldUser).Nullable();

            ManyToOne(x => x.University, c => {
                c.Column("UniversityId2");
                c.ForeignKey("User_University2");
                c.NotNullable(false);
            });
            //References(x => x.University).Column("UniversityId2").ForeignKey("User_University2").Nullable();

            Property(x => x.Language, c => {
                c.Column("Language");
                //c.Type<CultureInfo>();
                c.NotNullable(false);
            });
            //Map(x => x.Language).Column("Language").CustomType<CultureInfo>().Nullable();

            Bag(x => x.Questions, c => {
                c.Key(k =>
                {
                    k.ForeignKey("Question_User");
                    k.Column("UserId");
                });
                c.Inverse(true);
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Access(Accessor.ReadOnly | Accessor.NoSetter);
                c.Lazy(CollectionLazy.Lazy);
                //c.Table("Question");
            }, a => a.OneToMany(/*x => x.Class(typeof(Question))*/));

            //HasMany(x => x.Questions).Access.CamelCaseField(Prefix.Underscore)
            //    .Inverse()
            //    .Cascade.AllDeleteOrphan();
            
            Property(x => x.Score, c => {
                c.Access(Accessor.ReadOnly);
                c.Insert(false);
                c.Update(false);
            });
            //Map(x => x.Score).ReadOnly();
            Table("[User]");
            //Table("User"); //if not there is sql error

            DynamicUpdate(true);
            //DynamicUpdate();
            OptimisticLock(OptimisticLockMode.Version);
            //OptimisticLock.Version();

            Version(x => x.Version, c => {
                c.Generated(VersionGeneration.Always);
                c.Type(new BinaryBlobType());
                c.Column(cl => {
                    cl.SqlType("timestamp");
                });
            });

            //Version(x => x.Version).CustomSqlType("rowversion").Generated.Always();
            Discriminator(d => d.Column("Fictive"));
            //DiscriminateSubClassesOnColumn("Fictive");
            ///*
            // * CREATE UNIQUE NONCLUSTERED INDEX idx_phoneNumber_notnull
            //   ON sb.[User](PhoneNumberHash)
            //   WHERE PhoneNumberHash IS NOT NULL;
            // */
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Update);
            //SchemaAction.Update();
        }
    }


    public class FictiveUserMap : SubclassMapping<SystemUser>
    {
        public FictiveUserMap()
        {

            DiscriminatorValue(true);
        }
    }

}
