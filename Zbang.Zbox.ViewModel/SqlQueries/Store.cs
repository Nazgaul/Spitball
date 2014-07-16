using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
   public class Store
   {
       public const string GetProducts = @"SELECT [Id]
      ,[Name]
      ,[ExtraDetails]
      ,[NumberOfSales]
      ,[Coupon]
      ,[SalePrice]
      ,[PictureUrl]
        FROM [Zbox].[Product]";

       
   }
}
