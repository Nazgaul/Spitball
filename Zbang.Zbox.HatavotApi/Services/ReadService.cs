﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public class ReadService : IReadService
    {
        public async Task<IEnumerable<StoreDto>> ReadData()
        {
            using (var conn = await DapperConnection.OpenConnection("Hatavot"))
            {
                const string sql = @" select  [productid]  as Id -- Product ID 
      ,[name] -- Product Name 
      --,[description] -- Product Description (HTML) -- Product Page
      ,[saleprice] -- Regular Price 
      ,[image]-- Main Image (http://www.hatavot.co.il/uploadimages/250/XXX)
      --,[catcode] -- Categories (list to  CatCode in [bizpoin_bizpointDB].[categories])
      --,[featured] -- Show on main page 
      --,[show] -- Active if NOT 'ON'
      --,[CatalogNumber] -- part of Product Description 
      ,[SubName] as ExtraDetails-- 2nd title of the Product Description 
      --,[SupplyTime]-- part of Product description 
      --,[IndexProdOrder] -- the rank of the product on the page (lower is higher on the page_)
      --,[SalesProdOrder] -- sales page product order 
      --,[ProdOrder] -- Category Page  product order
      --,[ProducerId] -- Producer ID 
      --,[p1] -- Upgrades
      --,[v1]-- Upgrades
      --,[p2]-- Upgrades
      --,[v2]-- Upgrades
      --,[p3]-- Upgrades
      --,[v3]-- Upgrades
      --,[p4]-- Upgrades
      --,[v4]-- Upgrades
      --,[p5]-- Upgrades
      --,[v5]-- Upgrades
      --,[p6]-- Upgrades
      --,[v6]-- Upgrades
      --,[DeliveryPrice] -- Delivery charge 
      --,[ProductPayment]-- Number of payments 
      ,[coupon]-- Discount amount --> Student Price = [SalePrice] - [Coupon] 
      --,[designNum] -- Which University to show --> Can be to all or to one specific  
  FROM [bizpoin_bizpointDB].[products] where [show] is  null";
                return await conn.QueryAsync<StoreDto>(sql);
            }
        }
    }
}
