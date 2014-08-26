using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using System;
using System.Linq;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public class WriteService : IWriteService
    {
        private const string ConnectionStringName = "Hatavot";
        public OrderDto InsertOrder(OrderSubmitDto order)
        {
            using (var conn = DapperConnection.OpenConnection(ConnectionStringName))
            {
                var initSqls = new[]
                {
                    "SELECT MAX(Aorderid) + 7  AS max_Aorderid FROM orders",
                    "SELECT MAX(CustId) + 1 AS max_CustId FROM customers",
                    "SELECT MAX(orderitemsid)+1 AS max_orderitemsid FROM orderitems",
                    "SELECT MAX(ProductId)+1 AS max_ProductId FROM BackUpProducts",
                    "SELECT * FROM products WHERE products.productid = @ProductId" 
                };
                long currentOrderId, currentCustomerId, currentOrderItemId;
                int currentProductId;
                OrderProductDto product;
                using (var grid = conn.QueryMultiple(string.Join(";", initSqls), new { ProductId = order.ProdcutId }))
                {
                    currentOrderId = grid.Read<long>().FirstOrDefault();
                    currentCustomerId = grid.Read<long>().FirstOrDefault();
                    currentOrderItemId = grid.Read<long>().FirstOrDefault();
                    currentProductId = grid.Read<int>().FirstOrDefault();
                    product = grid.Read<OrderProductDto>().FirstOrDefault();
                }
                if (product == null)
                {
                    throw new NullReferenceException("product");
                }
                //insert values to BackUpProducts 
                conn.Execute(
                    @"INSERT INTO BackUpProducts (ProductId,name,price,saleprice,image,catcode,CatalogNumber,SupplyTime,ProdOrder,OrgProductId,
profitPercent,CurrencyType,Vat,ProducerId, referer)
VALUES ( @curr_ProductId, @products_name,@products_price,@products_saleprice,@products_image,@products_catcode,@products_CatalogNumber,@products_SupplyTime,
@products_ProdOrder,@products_productid,@products_ProfitPercent,@products_CurrencyType,@products_Vat,@products_ProducerId,@referer)"
                    , new
                    {
                        curr_ProductId = currentProductId,
                        products_name = product.Name,
                        products_price = product.SalePrice,
                        products_saleprice = product.SalePrice,
                        products_image = product.Image,
                        products_catcode = product.Catcode,
                        products_CatalogNumber = product.CatalogNumber,
                        products_SupplyTime = product.SupplyTime,
                        products_ProdOrder = product.ProdOrder,
                        products_productid = product.ProducerId,
                        products_ProfitPercent = product.ProfitPercent,
                        products_CurrencyType = product.CurrencyType,
                        products_Vat = product.Vat,
                        products_ProducerId = product.ProducerId,
                        referer = order.UniversityId
                    });

                var t = conn.Query<int>("SELECT MAX(ProductId) AS max_BackUpProducts_ProductId FROM BackUpProducts");
                var tmpOrgProductId = t.FirstOrDefault();
                var hashedCardHolderIdentificationNumber =
                    VbExternalUtils.Utils.Base64encode(order.CardHolderIdentificationNumber);
                var hashedCreditCardNumber = VbExternalUtils.Utils.Base64encode(order.CreditCardNumber);
                conn.Execute(
                    @"INSERT INTO orders (orderId,custid, StudentId, [date],dfname,dlname,daddress1,dstate,dcity,dzip,ccname, coupon, ids,ccnumber,
ccexpire,ccBehind,isphone,notes,Adminstatus,shopType,totalprice,delivery,SelfDeliver,NumOfPayment,referer, mosadName) 
VALUES (@orderId,@custid,@StudentCode,@the_date,@dfname,@dlname,@daddress1,@dstate,@dcity,@dzip,@ccname,@coupon,@ids,
@ccnumber,@ccexpire,@ccBehind,@isphone,@notes,@Adminstatus,@shopType,@totalprice,@delivery,@SelfDeliver,@NumOfPayment,@referer,@mosadName)",
                    new
                    {
                        orderId = currentOrderId,
                        custid = currentCustomerId,
                        StudentCode = order.IdentificationNumber,
                        the_date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        dfname = order.FirstName,
                        dlname = order.LastName,
                        daddress1 = order.Address,
                        dstate = string.Empty,
                        dcity = order.City,
                        dzip = string.Empty,
                        ccname = order.CreditCardNameHolder,
                        coupon = product.Coupon,
                        ids = hashedCardHolderIdentificationNumber, // to hash
                        ccnumber = hashedCreditCardNumber, // to hash
                        ccexpire = order.CreditCardExpiration.ToString("MMyy"),
                        ccBehind = order.Cvv,
                        isphone = 0,
                        notes = order.Notes,
                        Adminstatus = 1,
                        shopType = 1,
                        totalprice = (product.SalePrice + product.DeliveryPrice) *10,
                        delivery = 0,
                        SelfDeliver = 0,
                        NumOfPayment = 1,
                        referer = order.UniversityId,
                        mosadName = string.Empty
                    });




                conn.Execute(@"INSERT INTO orderitems (orderitemsid,orderid,productid,qty,priceperunit,p1,v1,p2,v2,p3,v3,p4,v4,p5,v5,p6,v6,
DeliveryPrice,AdditionPrice,productPayment,orderType) 
VALUES (@orderitemsid,@orderid,@productid,@qty,@priceperunit,@p1,@v1,@p2,@v2,@p3,@v3,@p4,@v4,@p5,@v5,@p6,@v6,@DeliveryPrice,@AdditionPrice,
@productPayment,@orderType)",
                    new
                    {
                        orderitemsid = currentOrderItemId,
                        orderid = currentOrderId,
                        productid = tmpOrgProductId,
                        qty = 1,
                        priceperunit = product.SalePrice,
                        p1 = order.P1,
                        v1 = order.V1,
                        p2 = order.P2,
                        v2 = order.V2,
                        p3 = order.P3,
                        v3 = order.V3,
                        p4 = order.P4,
                        v4 = order.V4,
                        p5 = order.P5,
                        v5 = order.V5,
                        p6 = order.P6,
                        v6 = order.V6,
                        product.DeliveryPrice,
                        AdditionPrice = order.ExtraFeaturePrice,
                        productPayment = order.NumberOfPayment,
                        orderType = 1

                    });

                conn.Execute(@"INSERT INTO customers (CustId, StudentId, fname,lname,email,address1,state,city,country,pass,phone,
Cphone1,fax, coupon, ids,OrgName,OrgNumber,isMailingList) 
VALUES (@CustId,@StudentCode,@fname,@lname,@email,@address1,@state,@city,@country,@pass,@phone,@Cphone1,@fax,@coupon,@cust_ids,@OrgName,@OrgNumber,
@isMailingList)", new
                {
                    CustId = currentCustomerId,
                    StudentCode = order.IdentificationNumber,
                    fname = order.FirstName,
                    lname = order.LastName,
                    email = order.Email,
                    address1 = order.Address,
                    state = string.Empty,
                    city = order.City,
                    country = "ישראל ישראל",
                    pass = 1234567,
                    phone = order.Phone,
                    Cphone1 = order.Phone2,
                    fax = string.Empty,
                    coupon = product.Coupon,
                    cust_ids = string.Empty,
                    OrgName = string.Empty,
                    OrgNumber = string.Empty,
                    isMailingList = 0
                });
                return new OrderDto(currentOrderId, product.Name);
            }

        }
    }
}
