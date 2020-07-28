//using System;
//using System.Diagnostics.CodeAnalysis;

//namespace Cloudents.Core
//{
//    [SuppressMessage("ReSharper", "UnusedMember.Local")]
//    public sealed class PointBundle : Enumeration
//    {
//        public static readonly PointBundle Point1500 = new PointBundle(1500, 60, -1);
//        public static readonly PointBundle Point750 = new PointBundle(750, 30, -1);
//        public static readonly PointBundle Point250 = new PointBundle(250, 10, -1);
//        public static readonly PointBundle Point1000 = new PointBundle(1000, 60, 10);
//        public static readonly PointBundle Point500 = new PointBundle(500, -1, 6);
//        public static readonly PointBundle Point100 = new PointBundle(100, -1, 1.5);
//        private readonly int _priceInIls;
//        private readonly double _priceInUsd;


//        //public static PointBundle Parse(int points)
//        //{
//        //    if (Point250.Points.Equals(points))
//        //    {
//        //        return Point250;
//        //    }
//        //    if (Point750.Points.Equals(points))
//        //    {
//        //        return Point750;
//        //    }
//        //    if (Point1500.Points.Equals(points))
//        //    {
//        //        return Point1500;
//        //    }
//        //    throw new ArgumentException();
//        //}
//        private PointBundle(int amount, int priceInILS, double priceInUSD) : base(amount, amount.ToString())
//        {
//            Points = amount;
//            _priceInIls = priceInILS;
//            _priceInUsd = priceInUSD;
//        }

//        public int PriceInILS
//        {
//            get
//            {
//                if (_priceInIls == -1)
//                {
//                    throw new ArgumentException();
//                }

//                return _priceInIls;
//            }
//        }


//        public double PriceInUsd
//        {
//            get
//            {
//                if (_priceInUsd == -1)
//                {
//                    throw new ArgumentException();
//                }

//                return _priceInUsd;
//            }
//        }

//        public int Points { get; }

//        //private bool Equals(PointBundle other)
//        //{
//        //    return PriceInILS == other.PriceInILS && Points == other.Points;
//        //}

//        //public override bool Equals(object obj)
//        //{
//        //    return ReferenceEquals(this, obj) || obj is PointBundle other && Equals(other);
//        //}

//        //public override int GetHashCode()
//        //{
//        //    unchecked
//        //    {
//        //        return (PriceInILS * 397) ^ Points.GetHashCode();
//        //    }
//        //}

//        //public static bool operator ==(PointBundle left, PointBundle right)
//        //{
//        //    return Equals(left, right);
//        //}

//        //public static bool operator !=(PointBundle left, PointBundle right)
//        //{
//        //    return !Equals(left, right);
//        //}
//    }
//}