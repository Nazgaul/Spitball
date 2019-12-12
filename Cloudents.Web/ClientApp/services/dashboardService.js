import { connectivityModule } from "./connectivity.module";
// import { Promise } from "q";

// let dummy = [
//    {preview:'./Desktop.png',info:'maor',date:"2019-12-09T14:33:51.3495317Z",status:"Paid",type:'Doc',price:56},
//    {preview:'./Desktop.png',info:'ron',date:"2019-12-09T14:33:51.6648492Z",status:"Pending Spitball",type:'Video',price:32},
//    {preview:'./Desktop.png',info:'eidan',date:"2019-12-08T12:35:31.9032542Z",status:"Pending Student",type:'Q&A',price:26},
//    {preview:'./Desktop.png',info:'gab',date:"2019-12-09T14:33:51.6648492Z",status:"Processing",type:'Doc',price:21},
//    {preview:'./Desktop.png',info:'elad',date:"2019-12-08T09:40:16.459378Z",status:"Declined",type:'Doc',price:16}
// ]

function SalesItem(objInit){
   this.id = objInit.id || null
   this.preview = objInit.preview || '';
   this.info = objInit.info || '';
   this.type = objInit.type || '';
   // this.likes = objInit.likes;
   // this.views = objInit.views;
   // this.downloads = objInit.downloads;
   // this.purchased = objInit.purchased;
   this.price = objInit.price;
   this.status = objInit.status || '';
   this.date = objInit.date || null;
   this.course = objInit.course;
   this.name = objInit.name || '';
   this.studentName = objInit.studentName || '';
   this.duration = objInit.duration;
}

function createSalesItem(objInit){
   return new SalesItem(objInit)
}

function createSalesItems({data}) {
   let salesItems = [];

   data.forEach(item => salesItems.push(createSalesItem(item)));

   return salesItems;
}

function getSalesItems(){
   return connectivityModule.http.get('/Account/sales').then(createSalesItems).catch(ex => ex)
   // return Promise.resolve(dummy);
   // return connectivityModule.http.get(`/Document/${id}`).then(({data})=>data.map( item => createSalesItem(item)));
}

export default {
   getSalesItems,
}
