using System;

using NHibernate.Id;

namespace Zbang.Zbox.Infrastructure.IdGenerator
{
    public class GuidIdGenerator : IGuidIdGenerator
    {
        public static Guid GetGuid()
        {
            var combGenerator = new GuidCombGenerator();
            return (Guid)combGenerator.Generate(null, null);
        }

        public static Guid GetGuid(DateTime time)
        {
            return GenerateComb(time);
        }

        private static Guid GenerateComb(DateTime time)
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();

            var baseDate = new DateTime(1900, 1, 1);
            var now = time;

            // Get the days and milliseconds which will be used to build the byte string 
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var msecs = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        public Guid GetId()
        {
            return GetGuid();
        }
    }
}
