import { connectivityModule } from "./connectivity.module";
function _itemTypeChcker(type){
   if(type === 'Document' || type === 'Video'){
      return 'Document';
   }
   if(type === 'Question' || type === 'Answer'){
      return 'Question';
   }
}

function SalesItem(objInit){
   return Object.assign(
      new DefaultItem(objInit),
      {
         name: objInit.name,
         price: objInit.price,
         course: objInit.course || '',
         preview: objInit.preview || '',
         id: objInit.id || null,
         duration: objInit.duration,
         answerText: objInit.answerText || '',
         text: objInit.text || '',
         studentImage: objInit.studentImage || '',
         studentName: objInit.studentName,
         studentId: objInit.studentId || '',
         url: objInit.url,
         paymentStatus: objInit.paymentStatus,
      }
   )
}
function DefaultItem(objInit){
   this.type = objInit.type;
   this.date = objInit.date;
}
const Item = {
   Default:function(objInit){
      this.type = objInit.type;
      this.date = objInit.date;
      this.course = objInit.course;
   },
   Document:function(objInit){
      this.name = objInit.name;
      this.price = objInit.price;
      this.views = objInit.views;
      this.likes = objInit.likes;
      this.downloads = objInit.downloads;
      this.purchased = objInit.purchased;
      this.preview = objInit.preview;
      this.url = objInit.url;
      this.id = objInit.id;
   },
   Question:function(objInit){
      this.id = objInit.questionId || objInit.id;
      this.text = objInit.text || objInit.questionText;
      this.answerText = objInit.answerText || '';
   },
   // Session:function(objInit){

   // }
}

function ContentItem(objInit){
   return Object.assign(
      new Item.Default(objInit),
      new Item[_itemTypeChcker(objInit.type)](objInit)
   )
}
// {
//     {
//     "id": 50479,
//     "name": "2dae",
//     "course": "Temp",
//     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/50479",
//     "url": "/document/temp/2dae/50479",
//     "type": "Document",
//     "price": -11,
//     "date": "2019-12-12T11:25:41.1461168Z"
//   },
//   {
//     "tutorName": "gab ha totach",
//     "tutorImage": "/image/ClVodHRwczovL3NwaXRiYWxsZGV2LmJsb2IuY29yZS53aW5kb3dzLm5ldC9zcGl0YmFsbC11c2VyL3Byb2ZpbGUvMTYwMTA1LzE1Njc5NDYwMTguanBnEAA",
//     "tutorId": 160105,
//     "type": "TutoringSession",
//     "date": "2019-12-04T15:19:52.64782Z"
//   },
//   {
//     "tutorName": "gab ha totach",
//     "duration": "00:00:17.2113312",
//     "tutorImage": "/image/ClVodHRwczovL3NwaXRiYWxsZGV2LmJsb2IuY29yZS53aW5kb3dzLm5ldC9zcGl0YmFsbC11c2VyL3Byb2ZpbGUvMTYwMTA1LzE1Njc5NDYwMTguanBnEAA",
//     "tutorId": 160105,
//     "type": "TutoringSession",
//     "price": 0,
//     "date": "2019-12-04T15:18:16.8458866Z"
//   },
//   {
//     "tutorName": "gab ha totach",
//     "duration": "00:02:16.6129951",
//     "tutorImage": "/image/ClVodHRwczovL3NwaXRiYWxsZGV2LmJsb2IuY29yZS53aW5kb3dzLm5ldC9zcGl0YmFsbC11c2VyL3Byb2ZpbGUvMTYwMTA1LzE1Njc5NDYwMTguanBnEAA",
//     "tutorId": 160105,
//     "type": "TutoringSession",
//     "price": 1.6667,
//     "date": "2019-12-04T15:15:43.4734697Z"
//   },
//   {
//     "tutorName": "gab ha totach",
//     "duration": "00:06:48.4517951",
//     "tutorImage": "/image/ClVodHRwczovL3NwaXRiYWxsZGV2LmJsb2IuY29yZS53aW5kb3dzLm5ldC9zcGl0YmFsbC11c2VyL3Byb2ZpbGUvMTYwMTA1LzE1Njc5NDYwMTguanBnEAA",
//     "tutorId": 160105,
//     "type": "TutoringSession",
//     "price": 5,
//     "date": "2019-12-04T15:02:32.950453Z"
//   },
//   {
//     "tutorName": "gab ha totach",
//     "duration": "00:00:13.5944467",
//     "tutorImage": "/image/ClVodHRwczovL3NwaXRiYWxsZGV2LmJsb2IuY29yZS53aW5kb3dzLm5ldC9zcGl0YmFsbC11c2VyL3Byb2ZpbGUvMTYwMTA1LzE1Njc5NDYwMTguanBnEAA",
//     "tutorId": 160105,
//     "type": "TutoringSession",
//     "price": 0,
//     "date": "2019-12-04T15:01:37.2798245Z"
//   },
//   {
//     "tutorName": "gab ha totach",
//     "duration": "00:00:14.6878245",
//     "tutorImage": "/image/ClVodHRwczovL3NwaXRiYWxsZGV2LmJsb2IuY29yZS53aW5kb3dzLm5ldC9zcGl0YmFsbC11c2VyL3Byb2ZpbGUvMTYwMTA1LzE1Njc5NDYwMTguanBnEAA",
//     "tutorId": 160105,
//     "type": "TutoringSession",
//     "price": 0,
//     "date": "2019-12-04T15:00:56.1368716Z"
//   },
//   {
//     "id": 49723,
//     "name": "sample",
//     "course": "Temp",
//     "preview": "https://spitball-dev-function.azureedge.net:443/api/image/document/49723",
//     "url": "/document/temp/sample/49723",
//     "type": "Video",
//     "price": -4,
//     "date": "2019-10-02T08:11:24.8537488Z"
//   },
// }
function PurchasesItem(objInit){
   
}

function createSalesItems({data}) {
   let salesItems = [];
   data.forEach(item => salesItems.push(new SalesItem(item)));
   return salesItems;
}
function createContentItems({data}) {
   let contentItems = [];
   data.map(item => contentItems.push(new ContentItem(item)));
   return contentItems;
}
function createPurchasesItems({data}) {
   let purchasesItems = [];
   data.map(item => purchasesItems.push(new PurchasesItem(item)));
   return purchasesItems;
}

function getSalesItems(){
   return connectivityModule.http.get('/Account/sales').then(createSalesItems).catch(ex => ex)
}
function getContentItems(){
   return connectivityModule.http.get('/Account/content').then(createContentItems).catch(ex => ex)
}
function getPurchasesItems(){
   return connectivityModule.http.get('/Account/purchases').then(createPurchasesItems).catch(ex => ex)
}

export default {
   getSalesItems,
   getContentItems,
   getPurchasesItems,
}
