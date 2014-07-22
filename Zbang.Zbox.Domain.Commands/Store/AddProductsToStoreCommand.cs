﻿using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Store
{
    public class AddProductsToStoreCommand : ICommand
    {
        public AddProductsToStoreCommand(IEnumerable<ProductStore> productStores)
        {
            ProductStores = productStores;
        }

        public IEnumerable<ProductStore> ProductStores { get; private set; }
    }

    public class ProductStore
    {
        public ProductStore(string catalogNumber, string categories, float coupon, float deliveryPrice, string description, 
            string extraDetails, bool featured, long id, string name, int numberOfSales, string pictureUrl, int productPayment,
            float salePrice, string supplyTime, string producerName)
        {
            CatalogNumber = catalogNumber;
            Categories = categories;
            Coupon = coupon;
            DeliveryPrice = deliveryPrice;
            Description = description;
            ExtraDetails = extraDetails;
            Featured = featured;
            Id = id;
            Name = name;
            NumberOfSales = numberOfSales;
            PictureUrl = pictureUrl;
            ProductPayment = productPayment;
            SalePrice = salePrice;
            SupplyTime = supplyTime;
            ProducerName = producerName;
        }

        public long Id { get;private set; }
        public string Name { get; private set; }
        public string ExtraDetails { get; private set; }

        public int NumberOfSales { get; private set; }

        public float Coupon { get; private set; }

        public float SalePrice { get; private set; }

        public string PictureUrl { get; private set; }

        public string Categories { get; private set; }


        public bool Featured { get; private set; }

        public string Description { get; private set; }
        public string SupplyTime { get; private set; }

        public int ProductPayment { get; private set; }

        public string CatalogNumber { get; private set; }
        public float DeliveryPrice { get; private set; }

        public string ProducerName { get; set; }
    }
}
