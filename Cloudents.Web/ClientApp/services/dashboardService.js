import { connectivityModule } from "./connectivity.module";

function itemTypeChcker(type){
   if(type.toLowerCase() === 'document' || type.toLowerCase() === 'video'){
      return 'Document';
   }
   if(type.toLowerCase() === 'question' || type.toLowerCase() === 'answer'){
      return 'Question';
   }
   if(type.toLowerCase() === 'tutoringsession'){
      return 'Session';
   }
   return console.error('type:',type,'is not defined')
}

const Item = {
   Default:function(objInit){
      this.type = objInit.type;
      this.date = objInit.date;
      this.course = objInit.course || '';
      this.id = objInit.id || objInit.questionId || objInit.tutorId || objInit.studentId;
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
   },
   Question:function(objInit){
      this.text = objInit.text || objInit.questionText;
      this.answerText = objInit.answerText || '';
   },
   Session:function(objInit){
      this.duration = objInit.duration;
      this.price = objInit.price;
      this.name = objInit.tutorName || objInit.studentName;
      this.image = objInit.tutorImage || objInit.studentImage;
   },
}
function ContentItem(objInit){
   return Object.assign(
      new Item.Default(objInit),
      new Item[itemTypeChcker(objInit.type)](objInit)
   )
}
function SalesItem(objInit){
   return Object.assign(
      new Item.Default(objInit),
      new Item[itemTypeChcker(objInit.type)](objInit),
      {
         paymentStatus: objInit.paymentStatus,
      }
   )
   
   
}
function PurchasesItem(objInit){
   return Object.assign(
      new Item.Default(objInit),
      new Item[itemTypeChcker(objInit.type)](objInit)
   )
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
