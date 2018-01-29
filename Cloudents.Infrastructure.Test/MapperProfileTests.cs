using AutoMapper;
using Cloudents.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class MapperProfileTests
    {
        [TestMethod]
        public void AssertConfigurationIsValid()
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new MapperProfile()));
            Mapper.Configuration.AssertConfigurationIsValid();

            Assert.IsTrue(true);
        }



        [TestMethod]
        public void IpConverterConvert_JsonWithGeoPoint_PointCreate()
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new MapperProfile()));
            var json =
                "{\"ip\":\"86.143.189.86\",\"country_code\":\"GB\",\"country_name\":\"United Kingdom\",\"region_code\":\"ENG\",\"region_name\":\"England\",\"city\":\"Rayleigh\",\"zip_code\":\"SS6\",\"time_zone\":\"Europe/London\",\"latitude\":51.5833,\"longitude\":0.6167,\"metro_code\":0}";
            var data = Mapper.Map<Location>(json);

            Assert.AreEqual(data.Point.Latitude, 51.5833);

        }
    }
}