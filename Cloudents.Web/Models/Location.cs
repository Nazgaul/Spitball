﻿//using System.Runtime.Serialization;
//using Cloudents.Core.Models;
//using JetBrains.Annotations;

//namespace Cloudents.Web.Models
//{
//    [DataContract]
//    public class LocationQuery
//    {
//        [DataMember(Order = 1), CanBeNull]
//        public GeographicCoordinate Point { get; set; }

//        [DataMember(Order = 2)]
//        internal Address Address { get; set; }

//        [DataMember(Order = 3)]
//        internal string Ip { get; set; }

//        [DataMember(Order = 4)]
//        public string CallingCode { get; set; }

//        public Location ToLocation()
//        {
//            return new Location(Point?.ToGeoPoint(), Address, Ip, CallingCode);
//        }
//    }
//}