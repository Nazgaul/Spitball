using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.Dapper;
using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public class WriteService
    {
        private const string ConnectionStringName = "Hatavot";
        public async void InsertOrder(OrderSubmitDto order)
        {
            using (var conn = await DapperConnection.OpenConnectionAsync(ConnectionStringName))
            {
                var initSqls = new[]
                {
                    "SELECT MAX(Aorderid) + 7  AS max_Aorderid FROM orders",
                    "SELECT MAX(CustId) + 1 AS max_CustId FROM customers",
                    "SELECT MAX(orderitemsid)+1 AS max_orderitemsid FROM orderitems",
                    "SELECT MAX(ProductId)+1 AS max_ProductId FROM BackUpProducts",
                    "SELECT * FROM products WHERE products.productid = @ProductId" 
                };
                long curr_orderId, curr_CustId, curr_orderitemsid, curr_ProductId;
                OrderProductDto product;
                using (var grid = await conn.QueryMultipleAsync(string.Join(";", initSqls), new { ProductId = order.ProdcutId }))
                {
                    curr_orderId = grid.Read<long>().FirstOrDefault();
                    curr_CustId = grid.Read<long>().FirstOrDefault();
                    curr_orderitemsid = grid.Read<long>().FirstOrDefault();
                    curr_ProductId = grid.Read<long>().FirstOrDefault();
                    product = grid.Read<OrderProductDto>().FirstOrDefault();
                }

                //insert values to BackUpProducts 
                await conn.ExecuteAsync(
                    @"INSERT INTO BackUpProducts (ProductId,name,price,saleprice,image,catcode,CatalogNumber,SupplyTime,ProdOrder,OrgProductId,profitPercent,CurrencyType,Vat,ProducerId, referer)
VALUES ( @curr_ProductId, @products_name,@products_price,@products_saleprice,@products_image,@products_catcode,@products_CatalogNumber,@products_SupplyTime,
@products_ProdOrder,@products_productid,@products_ProfitPercent,@products_CurrencyType,@products_Vat,@products_ProducerId,@referer)"
                    , new
                    {
                        curr_ProductId = curr_ProductId,
                        products_name = product.Name,
                        products_price = product.Price,
                        products_saleprice = product.Saleprice,
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

                var t = await conn.QueryAsync<int>("SELECT MAX(ProductId) AS max_BackUpProducts_ProductId FROM BackUpProducts");
                var tmpOrgProductId = t.FirstOrDefault();

                //orders table
//orderstatus = "a" ' ñèèåñ äæîðä 
//if request.form("SelfDeliver")="" then SelfDeliver=0 else SelfDeliver=request.form("SelfDeliver") ' äàí àéñåó òöîé

                await conn.ExecuteAsync(
                    @"INSERT INTO orders (orderId,custid, StudentId, [date],dfname,dlname,daddress1,dstate,dcity,dzip,ccname, coupon, ids,ccnumber,ccexpire,ccBehind,isphone,notes,Adminstatus,shopType,totalprice,delivery,SelfDeliver,NumOfPayment,referer, mosadName) 
VALUES (@orderId,@custid,@StudentCode,@the_date,@dfname,@dlname,@daddress1,@dstate,@dcity,@dzip,@ccname,@coupon,@ids,
@ccnumber,@ccexpire,@ccBehind,@isphone,@notes,@Adminstatus,@shopType,@totalprice,@delivery,@SelfDeliver,@NumOfPayment,@referer,@mosadName)",
                    new
                    {
                        orderId = curr_orderId,
                        custid = curr_CustId,
                        StudentCode = order.studentnum,
                        the_date = DateTime.UtcNow,
                        dfname = order.dfname,
                        dlname = order.dlname,
                        daddress1 = order.daddress1,
                        dstate = order.HouseNumber,
                        dcity = order.city,
                        dzip = order.zip,
                        ccname = order.ccname,
                        coupon = order.coupon,
                        ids = order.CardHolderIdentificationNumber,
                        ccnumber = order.ccNumber,
                        ccexpire = order.ccExpire.ToString("MMyy"),
                        ccBehind = order.cvv,
                        isphone = 0,
                        notes = order.Notes,
                        Adminstatus = 1,
                        shopType = 1, //buisness = ON means 2
                        totalprice = string.Empty,//order.lastPrice+ order.DeliveryPrice,
                        delivery = 0,
                        SelfDeliver = 0,
                        NumOfPayment = 1,
                        referer = order.UniversityId,
                        mosadName = order.mosadName
                    });
  


                conn.ExecuteAsync(@"INSERT INTO orderitems (orderitemsid,orderid,productid,qty,priceperunit,p1,v1,p2,v2,p3,v3,p4,v4,p5,v5,p6,v6,
DeliveryPrice,AdditionPrice,productPayment,orderType) 
VALUES (@orderitemsid,@orderid,@productid,@qty,@priceperunit,@p1,@v1,@p2,@v2,@p3,@v3,@p4,@v4,@p5,@v5,@p6,@v6,@DeliveryPrice,@AdditionPrice,
@productPayment,@orderType)",
                    new
                    {
                        orderitemsid = curr_orderitemsid,
                        orderid = curr_orderId,
                        productid = tmpOrgProductId,
                        qty = 1,
                        priceperunit = order.last_price,
                        p1 = order.p1,
                        v1 = order.v1,
                        p2 = order.p2,
                        v2 = order.v2,
                        p3 = order.p3,
                        v3 = order.v3,
                        p4 = order.p4,
                        v4 = order.v4,
                        p5 = order.p5,
                        v5 = order.v5,
                        p6 = order.p6,
                        v6 = order.v6,
                        DeliveryPrice = order.DeliveryPrice,
                        AdditionPrice =order.extraFeaturePrice,
                        productPayment = 1, //TODO: what is that
                        orderType = 1

                    });

                //        CustId = curr_CustId ' îñ ì÷åç
                //fname = request.form("dfname") ' ùí ôøèé
                //lname = request.form("dlname") ' ùí îùôçä
                //email = request.form("email") ' àéîééì
                //address1 =  request.form("daddress1") ' øçåá åîñôø
                //state = request.form("house_number") ' îñ áéú
                //city = request.form("dcity") ' òéø
                //country = "éùøàì éùøàì" ' îãéðä
                //pass = "1234567" ' ñéñîä
                //fax = request.form("CustNumber") ' îñ ùåáø àå îðåé
                //cust_ids = request.form("cust_ids") ' îñ úæ
                //phone = request.form("pre_phone") & "-" & request.form("phone") ' èìôåï
                //Cphone1 = request.form("pre_Cphone1") & "-" & request.form("Cphone1") ' èìôåï ðééã
                //OrgName = request.form("company_name") ' ùí çáøä
                //OrgNumber = request.form("company_number") ' îñ çáøä
                //if request.form("isMailingList")="1" then isMailingList = 1 else isMailingList = 0              

//strSQL="INSERT INTO customers (CustId, StudentId, fname,lname,email,address1,state,city,country,pass,phone,Cphone1,fax, coupon, ids,OrgName,OrgNumber,isMailingList) VALUES ("& CustId &",'"& StudentCode &"', '"& txtfix(fname) &"','"& txtfix(lname) &"','"& email &"','"& txtfix(address1) &"','"& txtfix(state) &"','"& txtfix(city) &"','"& country &"','"& pass &"','"& phone &"','"& Cphone1 &"','"& fax &"' ,"& coupon &",'"& cust_ids &"','"& txtfix(OrgName) &"','"& txtfix(OrgNumber) &"',"& isMailingList &")"
//r.open strSQL

                conn.ExecuteAsync(@"INSERT INTO customers (CustId, StudentId, fname,lname,email,address1,state,city,country,pass,phone,
Cphone1,fax, coupon, ids,OrgName,OrgNumber,isMailingList) 
VALUES (@CustId,@StudentCode,@fname,@lname,@email,@address1,@state,@city,@country,@pass,@phone,@Cphone1,@fax,@coupon,@cust_ids,@OrgName,@OrgNumber,
@isMailingList)", new
                {
                    CustId = curr_CustId,
                    StudentCode = order.SocialIdNumber ,
                    fname = order.FirstName,
                    lname = order.LastName,
                    email= order.Email,
                    address1= order.Address,
                    state =  order.HouseNumber,
                    city = order.city ,
                    country = "ישראל ישראל",
                    pass = 1234567,
                    phone = order.Phone,
                    Cphone1= order.Phone2,
                    fax= string.Empty,
                    coupon=order.coupon,
                    cust_ids= order.CardHolderIdentificationNumber,
                    OrgName= string.Empty,
                    OrgNumber= string.Empty,
                    isMailingList= 0
                });

//strSQL="INSERT INTO tbl_orders_archive (order_id,order_date,total_price,product_name,customer_name,referer, mosadName) VALUES ("& orderId &","& the_date &","& priceperunit + AdditionPrice &",'"& txtfix(products_name) &"','"& txtfix(dfname &" "& dlname) &"','"& referer &"', '"& mosadName &"')"
//r.open strSQL
//        //customer table



            }
        }
    }
}
