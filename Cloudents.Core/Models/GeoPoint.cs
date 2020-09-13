//using System.Runtime.Serialization;

//namespace Cloudents.Core.Models
//{
//    [DataContract]
//    public class GeoPoint
//    {
//        public GeoPoint(float longitude, float latitude)
//        {
//            Longitude = longitude;
//            Latitude = latitude;
//        }

//        protected GeoPoint()
//        {
//        }

//        public override bool Equals(object obj)
//        {
//            if (obj is null) return false;
//            if (ReferenceEquals(this, obj)) return true;
//            if (obj.GetType() != GetType()) return false;

//            var other = (GeoPoint)obj;
//            return Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude);
//        }

//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                return (Longitude.GetHashCode() * 11) ^ Latitude.GetHashCode();
//            }
//        }

//        /// <summary>
//        /// Longitude of user
//        /// </summary>
//        [DataMember(Order = 1)]
//        public float Longitude { get; /*[UsedImplicitly] set;*/ }

//        /// <summary>
//        /// Latitude of user
//        /// </summary>
//        [DataMember(Order = 2)]
//        public float Latitude { get;/* [UsedImplicitly] set;*/ }

//        public static bool operator ==(GeoPoint obj1, GeoPoint obj2)
//        {
//            if (ReferenceEquals(obj1, null))
//            {
//                return ReferenceEquals(obj2, null);
//            }

//            return obj1.Equals(obj2);
//        }

//        public static bool operator !=(GeoPoint obj1, GeoPoint obj2)
//        {
//            return !(obj1 == obj2);
//        }

//        //public GeographyPoint ToPoint()
//        //{
//        //    return GeographyPoint.Create(Latitude, Longitude);
//        //}
//    }
//}