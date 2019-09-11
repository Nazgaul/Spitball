using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Query.Payment;

namespace Cloudents.Core.Interfaces
{
    public interface IPayment
    {
        //Task<PayMeSellerResponse> CreateSellerAsync(PayMeSeller seller, CancellationToken token);
        Task<GenerateSaleResponse> CreateBuyerAsync(string callback,string successRedirect, CancellationToken token);

        Task<GenerateSaleResponse> TransferPaymentAsync(string sellerKey, string buyerKey,decimal price, CancellationToken token);

        Task<GenerateSaleResponse> BuyTokens(PointBundle price, string successRedirect,CancellationToken token);
    }

    public sealed class PointBundle
    {
        private static readonly PointBundle PaymentThree = new PointBundle(1500, 60);
        private static readonly PointBundle PaymentTwo = new PointBundle(750, 30);
        private static readonly PointBundle PaymentOne = new PointBundle(250, 10);


        public static  PointBundle Parse(int points)
        {
            if (PaymentOne.Points.Equals(points))
            {
                return PaymentOne;
            }
            if (PaymentTwo.Points.Equals(points))
            {
                return PaymentTwo;
            }
            if (PaymentThree.Points.Equals(points))
            {
                return PaymentThree;
            }
            throw new ArgumentException();
        }
        private PointBundle(int amount, int priceInShekel)
        {
            Points = amount;
            Price = priceInShekel;
        }

        public int Price { get;  }

        public int Points { get; }

        private bool Equals(PointBundle other)
        {
            return Price == other.Price && Points == other.Points;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is PointBundle other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Price * 397) ^ Points.GetHashCode();
            }
        }

        public static bool operator ==(PointBundle left, PointBundle right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PointBundle left, PointBundle right)
        {
            return !Equals(left, right);
        }
    }
}