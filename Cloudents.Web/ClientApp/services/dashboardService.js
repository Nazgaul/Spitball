import { connectivityModule } from "./connectivity.module";

function DefaultItem(objInit){
   this.type = objInit.type;
   this.date = objInit.date;
   this.status = objInit.status;
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
      }
   )
}

function ContentItem(objInit){
   this.preview = objInit.preview || '';
   this.likes = objInit.likes;
   this.views = objInit.views;
   this.downloads = objInit.downloads;
   this.purchased = objInit.purchased;
   this.price = objInit.price;
   this.url = objInit.url;
   this.text = objInit.text || '';
   this.answerText = objInit.answerText || '';
   this.course = objInit.course || '';
   this.name = objInit.name;
   this.id = objInit.id || null;
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
