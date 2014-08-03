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
                        referer = 15 // TODO - what is that
                    });

                var t = await conn.QueryAsync<int>("SELECT MAX(ProductId) AS max_BackUpProducts_ProductId FROM BackUpProducts");
                var tmpOrgProductId = t.FirstOrDefault();

                //orders table
//orderstatus = "a" ' ñèèåñ äæîðä 
//totalprice = request.form("last_price") + request.form("DeliveryPrice") ' ñäë îçéø
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
                        ids = order.ids,
                        ccnumber = order.ccNumber,
                        ccexpire = order.ccExpire.ToString("MMyy"),
                        ccBehind = order.cvv,
                        isphone = 0,
                        notes = order.Notes,
                        Adminstatus = 1,
                        shopType = 1, //buisness = ON means 2
                      //  totalprice,
                        delivery = 0,
                      //  SelfDeliver,
                        NumOfPayment = 1,
                      //  referer,
                        mosadName = order.mosadName

                                                     
                    });
                                                                                                                                           


                //orderitems table

//    orderitemsid = curr_orderitemsid ' ñôéøú äæîðú îåöø 
//orderid = curr_orderId ' îñ äæîðä
//productid = tmpOrgProductId
//orgProductId = request.form("productid") ' îñ îåöø
//qty = 1 ' ëîåú
//priceperunit = request.form("last_price") ' îçéø ìéçéãä
//DeliveryPrice = request.form("DeliveryPrice") ' îçéø îùìåç
//productPayment =  request.form("productPayment")' îñ úùìåîéí áôåòì
//    if productPayment="" then productPayment=1 
//        orderType = 1 ' ñåâ äæîðä
//p1 = request.form("p1") ' ñåâ úåñôú 1
//v1 = request.form("v1") ' úåñôú ùðáçøä 1
//    StrAddition1 = 0
//    if len(v1) > 1 AND instr(v1,"*") > 0 then 
//    StrAddition1 = cint(mid(v1,instr(v1,"*")+1,(instrrev(v1,"*")-instr(v1,"*"))-1))
//    End if
//p2 = request.form("p2") ' ñåâ úåñôú 2
//v2 = request.form("v2") ' úåñôú ùðáçøä 2
//    StrAddition2 = 0
//    if len(v2) > 1 AND instr(v2,"*") > 0 then 
//    StrAddition2 = cint(mid(v2,instr(v2,"*")+1,(instrrev(v2,"*")-instr(v2,"*"))-1))
//    End if
//p3 = request.form("p3") ' ñåâ úåñôú 3
//v3 = request.form("v3") ' úåñôú ùðáçøä 3
//    StrAddition3 = 0
//    if len(v3) > 1 AND instr(v3,"*") > 0 then 
//    StrAddition3 = cint(mid(v3,instr(v3,"*")+1,(instrrev(v3,"*")-instr(v3,"*"))-1))
//    End if
//p4 = request.form("p4") ' ñåâ úåñôú 4
//v4 = request.form("v4") ' úåñôú ùðáçøä 4
//    StrAddition4 = 0
//    if len(v4) > 1 AND instr(v4,"*") > 0 then 
//    StrAddition4 = cint(mid(v4,instr(v4,"*")+1,(instrrev(v4,"*")-instr(v4,"*"))-1))
//    End if
//p5 = request.form("p5") ' ñåâ úåñôú 5
//v5 = request.form("v5") ' úåñôú ùðáçøä 5
//    StrAddition5 = 0
//    if len(v5) > 1 AND instr(v5,"*") > 0 then 
//    StrAddition5 = cint(mid(v5,instr(v5,"*")+1,(instrrev(v5,"*")-instr(v5,"*"))-1))
//    End if
//p6 = request.form("p6") ' ñåâ úåñôú 6
//v6 = request.form("v6") ' úåñôú ùðáçøä 6
//    StrAddition6 = 0
//    if len(v6) > 1 AND instr(v6,"*") > 0 then 
//    StrAddition6 = cint(mid(v6,instr(v6,"*")+1,(instrrev(v6,"*")-instr(v6,"*"))-1))
//    End if
//AdditionPrice = StrAddition1 + StrAddition2 + StrAddition3 + StrAddition4 + StrAddition5 + StrAddition6 

//        //customer table

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

            }
        }
    }
}
