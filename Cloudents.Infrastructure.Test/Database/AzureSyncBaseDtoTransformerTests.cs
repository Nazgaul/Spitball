using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Query.Stuff;
using FluentAssertions;
using System;
using Xunit;

namespace Cloudents.Infrastructure.Test.Database
{
    public class AzureSyncBaseDtoTransformerTests
    {
        private enum ResultEnum
        {
            //Test1,
            Test2
        }

        private class ResultTest
        {
            public string Id { get; set; }
            public ResultEnum SomeEnum { get; set; }

            public ResultEnum? NullableEnum { get; set; }

        }
        [Fact]
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




        [Fact]
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


        [Fact]
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

        [Fact]
        public void TransformTuple_IntToEnum_ReturnEnum()
        {
            var x = new AzureSyncBaseDtoTransformer<AzureSyncBaseDto<ResultTest>, ResultTest>();

            object[] arr = new object[]
            {
                (int)ResultEnum.Test2

            };

            string[] alias = new string[]
            {
                nameof(AzureSyncBaseDto<ResultTest>.Data.SomeEnum)

            };
            var result = (AzureSyncBaseDto<ResultTest>)x.TransformTuple(arr, alias);

            result.Data.SomeEnum.Should().Be(ResultEnum.Test2);
        }


        [Fact]
        public void TransformTuple_StringToNullableEnum_ReturnEnum()
        {
            var x = new AzureSyncBaseDtoTransformer<AzureSyncBaseDto<ResultTest>, ResultTest>();

            object[] arr = new object[]
            {
                "Test2"

            };

            string[] alias = new string[]
            {
                nameof(AzureSyncBaseDto<ResultTest>.Data.NullableEnum)

            };
            var result = (AzureSyncBaseDto<ResultTest>)x.TransformTuple(arr, alias);

            result.Data.NullableEnum.Should().Be(ResultEnum.Test2);
        }
    }
}