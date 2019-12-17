import { connectivityModule } from "./connectivity.module";
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
   }
}








function ContentItem(objInit){
   if(objInit.type === 'Document' || objInit.type === 'Video'){
      return Object.assign(
         new Item.Default(objInit),
         new Item.Document(objInit)
      )
   }
   if(objInit.type === 'Question' || objInit.type === 'Answer'){
      return Object.assign(
         new Item.Default(objInit),
         new Item.Question(objInit),
      )
   }
}

function createSalesItems({data}) {
   let salesItems = [];
   data.forEach(item => salesItems.push(new SalesItem(item)));
   return salesItems;
}
function createContentItems({data}) {
   let contentItems = [];
   data.forEach(item => contentItems.push(new ContentItem(item)));
   return contentItems;
}

function getSalesItems(){
   return connectivityModule.http.get('/Account/sales').then(createSalesItems).catch(ex => ex)
}
function getContentItems(){
   return connectivityModule.http.get('/Account/content').then(createContentItems).catch(ex => ex)
}

export default {
   getSalesItems,
   getContentItems
}
