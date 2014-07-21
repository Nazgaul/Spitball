
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
        FROM [Zbox].[StoreProduct]";

       public const string GetProductsWithCategory = @"SELECT s.[ProductId] as Id
      ,[Name]
      ,[ExtraDetails]
      ,[NumberOfSales]
      ,[Coupon]
      ,[SalePrice]
      ,[PictureUrl]
      ,[Url]
  FROM [Zbox].[StoreProduct] s inner join zbox.StoreProductCategory sp on s.ProductId = sp.ProductId
  where sp.CatId = @CatId";

       public const string GetCategories = @"SELECT  catid as id, parentid, name, url as Url
  FROM [Zbox].[Zbox].[StoreCat]
  order by catorder";
   }
}
