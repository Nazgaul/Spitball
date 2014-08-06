using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public class ReadService : IReadService
    {
        private const string ConnectionStringName = "Hatavot";

        public IEnumerable<ProductDto> ReadData(int category)
        {
            using (var conn = DapperConnection.OpenConnection(ConnectionStringName))
            {
                const string sql = @"select [productid]  as Id -- Product ID 
      ,[name] -- Product Name 
      ,[description] -- Product Description (HTML) -- Product Page
      ,[saleprice] -- Regular Price 
      ,[image]-- Main Image (http://www.hatavot.co.il/uploadimages/250/XXX)
      ,[catcode] as CategoryCode -- Categories (list to  CatCode in [bizpoin_bizpointDB].[categories])
      ,[featured] -- Show on main page 
      ,[show] as NotActive -- Active if NOT 'ON'
      ,[CatalogNumber] -- part of Product Description 
      ,[SubName] as ExtraDetails-- 2nd title of the Product Description 
      ,[SupplyTime]-- part of Product description 
      --,[IndexProdOrder] -- the rank of the product on the page (lower is higher on the page_)
      --,[SalesProdOrder] -- sales page product order 
      --,[ProdOrder] -- Category Page  product order
      ,(select producerName from tblproducers s where s.producerid = p.[ProducerId]) as producerName -- Producer ID 
      ,[p1] as upgrade1  -- Upgrades
      ,[v1] as upgradeValue1 -- Upgrades
      ,[p2] as upgrade2 -- Upgrades
      ,[v2] as upgradeValue2-- Upgrades
      ,[p3] as upgrade3 -- Upgrades
      ,[v3] as upgradeValue3-- Upgrades
      ,[p4] as upgrade4 -- Upgrades
      ,[v4] as upgradeValue4-- Upgrades
      ,[p5] as upgrade5 -- Upgrades
      ,[v5] as upgradeValue5-- Upgrades
      ,[p6] as upgrade6 -- Upgrades
      ,[v6] as upgradeValue6-- Upgrades
      ,[DeliveryPrice] -- Delivery charge 
      ,[ProductPayment]-- Number of payments 
      ,[coupon]-- Discount amount --> Student Price = [SalePrice] - [Coupon] 
      ,[designNum] as UniversityId -- Which University to show --> Can be to all or to one specific  
  FROM [bizpoin_bizpointDB].[products] p where [show] is  null and catcode like '%' + cast( @catId as varchar) + '%'";
                return conn.Query<ProductDto>(sql, new { catId = category });
            }
        }

        public IEnumerable<CategoryDto> GetCategories()
        {
            using (var conn = DapperConnection.OpenConnection(ConnectionStringName))
            {
                return conn.Query<CategoryDto>(@"WITH cte 
AS
(
    select catcode,catname,parentid,catorder,1 as level from categories c where parentid = 611
    UNION ALL
    SELECT e.catcode,e.catname,e.parentid,e.catorder ,Level + 1
       
    FROM categories AS e
    INNER JOIN cte AS d
        ON e.parentid = d.catcode
)
select catcode as Id, catname as name, parentid, catorder as [order] from cte where level = 2");
            }
        }


        public IEnumerable<BannerDto> GetBanners()
        {
            using (var conn = DapperConnection.OpenConnection(ConnectionStringName))
            {

                var sql = @"SELECT top 1 [AdvID] as Id -- ID 
      ,[Image] -- URL http://hatavot.co.il/uploadimages/banners2/XXXX
      ,[AdvLink] as Url -- URL to point the banner if NULL static image
      --,[AdvOrder] -- University ID
      ,[BanOrder] as [Order] -- 1-9 Top-Right - 1, 10-19 Top-left (rotating), 20-29 Center - 2, 30-39 Top Product Page -1
  FROM [bizpoin_bizpointDB].[TblBanners]   where [PosID] = 2 and banorder between 1 and 9
  order by banorder

  SELECT [AdvID] as Id -- ID 
      ,[Image] -- URL http://hatavot.co.il/uploadimages/banners2/XXXX
      ,[AdvLink] as Url -- URL to point the banner if NULL static image
      --,[AdvOrder] -- University ID
      ,[BanOrder] as [Order] -- 1-9 Top-Right - 1, 10-19 Top-left (rotating), 20-29 Center - 2, 30-39 Top Product Page -1
  FROM [bizpoin_bizpointDB].[TblBanners]   where [PosID] = 2 and banorder between 10 and 19

  SELECT top 2 [AdvID] as Id -- ID 
      ,[Image] -- URL http://hatavot.co.il/uploadimages/banners2/XXXX
      ,[AdvLink] as Url -- URL to point the banner if NULL static image
      --,[AdvOrder] -- University ID
      ,[BanOrder] as [Order] -- 1-9 Top-Right - 1, 10-19 Top-left (rotating), 20-29 Center - 2, 30-39 Top Product Page -1
  FROM [bizpoin_bizpointDB].[TblBanners]   where [PosID] = 2 and banorder between 20 and 29
  order by banorder

  SELECT top 1 [AdvID] as Id -- ID 
      ,[Image] -- URL http://hatavot.co.il/uploadimages/banners2/XXXX
      ,[AdvLink] as Url -- URL to point the banner if NULL static image
      --,[AdvOrder] -- University ID
      ,[BanOrder] as [Order] -- 1-9 Top-Right - 1, 10-19 Top-left (rotating), 20-29 Center - 2, 30-39 Top Product Page -1
  FROM [bizpoin_bizpointDB].[TblBanners]   where [PosID] = 2 and banorder between 30 and 39
  order by banorder";
                using (var grid = conn.QueryMultiple(sql))
                {
                    var retVal = grid.Read<BannerDto>();
                    retVal = retVal.Union(grid.Read<BannerDto>());
                    retVal = retVal.Union(grid.Read<BannerDto>());
                    retVal = retVal.Union(grid.Read<BannerDto>());
                    return retVal;
                }

            }
        }
    }
}
