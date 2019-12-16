import { connectivityModule } from "./connectivity.module";

function DefaultItem(objInit){
   this.type = objInit.type;
   this.date = objInit.date;
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

function ContentItem(objInit){
   return Object.assign(
      new DefaultItem(objInit),
      {
         preview: objInit.preview || '',
         likes: objInit.likes,
         views: objInit.views,
         downloads: objInit.downloads,
         purchased: objInit.purchased,
         price: objInit.price,
         url: objInit.url,
         text: objInit.text || '',
         answerText: objInit.answerText || '',
         course: objInit.course || '',
         name: objInit.name,
         id: objInit.id || null,
         // state: objInit.state,
      }
   )
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
