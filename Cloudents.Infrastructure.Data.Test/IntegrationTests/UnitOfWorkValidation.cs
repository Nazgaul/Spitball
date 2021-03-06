﻿using Autofac.Extras.Moq;
using Cloudents.Core;
using Cloudents.Persistence;
using System;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    public class UnitOfWorkValidation
    {
        [Fact]
        public void ValidateDataBaseDevSchema()
        {
            using var mock = AutoMock.GetLoose();
            try
            {
                var configuration = new ConfigurationKeys()
                {
                    Db = new DbConnectionString(
                        "Server=tcp:sb-dev.database.windows.net,1433;Initial Catalog=ZboxNew_Develop;Persist Security Info=False;User ID=sb-dev;Password=Pa$$W0rd123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                        null, DbConnectionString.DataBaseIntegration.Validate)

                    //PROD
                    //Db = new DbConnectionString(
                    //"Server=tcp:on0rodxe8f.database.windows.net,1433;Initial Catalog=ZboxNew;Persist Security Info=False;User ID=ZBoxAdmin@on0rodxe8f;Password=Pa$$W0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                    //null, DbConnectionString.DataBaseIntegration.Validate)

                };
                var moq = mock.Create<PublishEventsListener>();
                var unitOfWorkSpitball = new UnitOfWorkFactorySpitball(moq, null, configuration);
                unitOfWorkSpitball.OpenSession();
            }
            catch (FluentNHibernate.Cfg.FluentConfigurationException ex) when (ex.InnerException?.GetType() == typeof(NHibernate.SchemaValidationException))
            {
                var innerEx = (NHibernate.SchemaValidationException)ex.InnerException;
                Assert.False(true, string.Join(Environment.NewLine, innerEx.ValidationErrors));

            }
        }

        [Fact(Skip = "No need to run this all build because it will fail")]
        public void ValidateDataBaseProdSchema()
        {
            using var mock = AutoMock.GetLoose();
            try
            {
                var configuration = new ConfigurationKeys()
                {
                    //Db = new DbConnectionString(
                    //    "Server=tcp:sb-dev.database.windows.net,1433;Initial Catalog=ZboxNew_Develop;Persist Security Info=False;User ID=sb-dev;Password=Pa$$W0rd123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                    //    null, DbConnectionString.DataBaseIntegration.Validate)

                    //PROD
                    Db = new DbConnectionString(
                        "Server=tcp:on0rodxe8f.database.windows.net,1433;Initial Catalog=ZboxNew;Persist Security Info=False;User ID=ZBoxAdmin@on0rodxe8f;Password=Pa$$W0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                        null, DbConnectionString.DataBaseIntegration.Validate)

                };
                var moq = mock.Create<PublishEventsListener>();
                var unitOfWorkSpitball = new UnitOfWorkFactorySpitball(moq, null, configuration);
                unitOfWorkSpitball.OpenSession();
            }
            catch (FluentNHibernate.Cfg.FluentConfigurationException ex) when (ex.InnerException?.GetType() == typeof(NHibernate.SchemaValidationException))
            {
                var innerEx = (NHibernate.SchemaValidationException)ex.InnerException;
                Assert.False(true, string.Join(Environment.NewLine, innerEx.ValidationErrors));

            }
        }
    }
}