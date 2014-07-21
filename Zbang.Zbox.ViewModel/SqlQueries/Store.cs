
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
        FROM [Zbox].[StoreProduct]";

       public const string GetCategories = @"SELECT  catid as id, parentid, name
  FROM [Zbox].[Zbox].[StoreCat]
  order by catorder";
   }
}
