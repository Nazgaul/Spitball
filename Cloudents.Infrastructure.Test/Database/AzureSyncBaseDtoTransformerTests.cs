using System;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Infrastructure.Database;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test.Database
{
    [TestClass]
    public class AzureSyncBaseDtoTransformerTests
    {

        class ResultTest
        {
            public string Id { get; set; }
        }
        [TestMethod]
        public void TransformTuple_GuidCheck_ReturnGuidString()
        {
            var x = new AzureSyncBaseDtoTransformer<AzureSyncBaseDto<ResultTest>, ResultTest>();

            object[] arr = new object[]
            {
                Guid.Empty

            };

            string[] alias = new string[]
            {
                nameof(ResultTest.Id)

            };
            var result = (AzureSyncBaseDto<ResultTest>)x.TransformTuple(arr, alias);

            result.Id.Should().Be(Guid.Empty.ToString());
            result.Data.Id.Should().Be(Guid.Empty.ToString());
        }



     
        [TestMethod]
        public void TransformTuple_LongCheck_ReturnLongString()
        {
            var x = new AzureSyncBaseDtoTransformer<AzureSyncBaseDto<ResultTest>, ResultTest>();

            object[] arr = new object[]
            {
                1L

            };

            string[] alias = new string[]
            {
                nameof(ResultTest.Id)

            };
            var result = (AzureSyncBaseDto<ResultTest>)x.TransformTuple(arr, alias);

            result.Id.Should().Be("1");
            result.Data.Id.Should().Be("1");
        }


        [TestMethod]
        public void TransformTuple_NullableLongCheck_ReturnLongString()
        {
            var x = new AzureSyncBaseDtoTransformer<AzureSyncBaseDto<ResultTest>, ResultTest>();

            object[] arr = new object[]
            {
                null

            };

            string[] alias = new string[]
            {
                nameof(AzureSyncBaseDto<ResultTest>.SYS_CHANGE_VERSION)

            };
            var result = (AzureSyncBaseDto<ResultTest>)x.TransformTuple(arr, alias);

            result.SYS_CHANGE_VERSION.Should().BeNull();
        }
    }
}