import { connectivityModule } from "./connectivity.module";

function SalesItem(objInit){
   this.name = objInit.name;
   this.price = objInit.price;
   this.course = objInit.course || '';
   this.id = objInit.id || null;
   this.preview = objInit.preview || '';
   this.type = objInit.type || '';
   this.status = objInit.status || '';
   this.date = objInit.date || null;
   this.studentName = objInit.studentName;
   this.duration = objInit.duration;
   this.answerText = objInit.answerText || '';
   this.text = objInit.text || '';
   this.studentImage = objInit.studentImage || '';
   this.studentId = objInit.studentId || '';
   this.url = objInit.url
}

// function ContentItem(objInit){
//    this.id = objInit.id || null
//    this.preview = objInit.preview || '';
//    this.info = objInit.info || '';
//    this.type = objInit.type || '';
//    this.likes = objInit.likes;
//    this.views = objInit.views;
//    this.downloads = objInit.downloads;
//    this.purchased = objInit.purchased;
//    this.price = objInit.price;
//    this.status = objInit.status || '';
//    this.date = objInit.date || null;
// }

function createSalesItems({data}) {
   let salesItems = [];
   data.forEach(item => salesItems.push(new SalesItem(item)));
   return salesItems;
}
// function createContentItems({data}) {
//    let contentItems = [];
//    data.forEach(item => contentItems.push(new ContentItem(item)));
//    return contentItems;
// }

function getSalesItems(){
   return connectivityModule.http.get('/Account/sales').then(createSalesItems).catch(ex => ex)
}
// function getContentItems(){
//    return connectivityModule.http.get('/Account/content').then(createContentItems).catch(ex => ex)
// }

export default {
   getSalesItems,
   // getContentItems
}
