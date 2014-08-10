
namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public static class Store
   {
       public const string GetProducts = @"SELECT [ProductId] as Id
      ,[Name]
      ,[ExtraDetails]
      ,[NumberOfSales]
      ,[Coupon]
      ,[SalePrice]
      ,[PictureUrl]
      ,[Url]
        FROM [Zbox].[StoreProduct] where homepage = 1
        and (universityid is null or universityid = @universityId)
        order by ProductOrder, SalePrice";

       public const string GetProductsWithCategory = @"SELECT s.[ProductId] as Id
      ,[Name]
      ,[ExtraDetails]
      ,[NumberOfSales]
      ,[Coupon]
      ,[SalePrice]
      ,[PictureUrl]
      ,[Url]
  FROM [Zbox].[StoreProduct] s inner join zbox.StoreProductCategory sp on s.ProductId = sp.ProductId
  where sp.CatId = @CatId
        and (universityid is null or universityid = @universityId)
        order by categoryorder, SalePrice";

       public const string GetCategories = @"SELECT  catid as id, parentid, name, url as Url
  FROM [Zbox].[StoreCat]
  order by catorder";

       public const string GetProduct = @"SELECT Name,ExtraDetails,description, SalePrice - Coupon as price, 
deliveryprice,catalognumber,numberofpayments,supplytime, [PictureUrl] as picture
  FROM [Zbox].[StoreProduct] s where s.ProductId = @ProdId";

       public const string GetProductCheckOut = @"SELECT 
Name,
ExtraDetails,
SalePrice,
Coupon,
[PictureUrl] as picture,
s.DeliveryPrice,
s.numberofpayments as NumberOfPayments
  FROM [Zbox].[StoreProduct] s where s.ProductId = @ProdId;";

       public const string SearchProduct = @"select [ProductId] as Id
      ,[Name]
      ,[ExtraDetails]
      ,[NumberOfSales]
      ,[Coupon]
      ,[SalePrice]
      ,[PictureUrl]
      ,[Url] from zbox.StoreProduct 
where name like '%' + @term + '%' 
and (universityid is null or universityid = @universityId)
order by ProductOrder, SalePrice";

       public const string GetProductFeatures = @"select Id, 
Category,
Description,
Price 
from zbox.StoreProductFeatures where Product = @ProdId;";

       public const string GetBanners = @"SELECT [Url]
      ,[ImageUrl]
      ,[Location]
      ,[BannerOrder]
  FROM [Zbox].[StoreBanner] where universityid in (210,@universityId) order by universityId";


       public const string ValidateCouponCode = "select 1 from zbox.StoreUniversityMapper where CouponCode = @Coupun";

       public const string MapCloudentsUniversityToStoreUniversity =
           "select StoreUniversityId from zbox.StoreUniversityMapper where UniversityId = @UniversityId";
   }
}
