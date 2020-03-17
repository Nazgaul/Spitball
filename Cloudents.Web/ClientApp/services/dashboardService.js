import { connectivityModule } from "./connectivity.module";
import { Blogs } from './Dto/blogs'

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
   if(type.toLowerCase() === 'buypoints'){
      return 'BuyPoints';
   }
   return console.error('type:',type,'is not defined');
}

function buildSessionDuration(totalMinutes) {
   let hours = Math.floor(totalMinutes / 60)
   let minutes = Math.floor(totalMinutes % 60)
   return `${hours.toString().padStart(2,0)}:${minutes.toString().padStart(2,0)}:00`;
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
      this.sessionId = objInit.sessionId
      this.price = objInit.price;
      this.name = objInit.tutorName || objInit.studentName;
      this.image = objInit.tutorImage || objInit.studentImage;
      this.totalMinutes = Math.floor(objInit.totalMinutes)
      this.duration = buildSessionDuration(objInit.totalMinutes)
   },
   StudyRoom:function(objInit){
      this.online = objInit.online;
      this.id = objInit.id;
      this.conversationId = objInit.conversationId;
      this.lastSession = objInit.lastSession;
   },
   BuyPoints: function(objInit){
      this.price = objInit.price;
      this.image = require('../components/pages/dashboardPage/mySales/buyPointsLayout/image/cardBuyPoints.jpg');
   },
   User:function(objInit){
      this.name = objInit.name;
      this.image = objInit.image;
      this.userId = objInit.userId;
      this.date = objInit.dateTime || objInit.created;
   },
   Follower:function(objInit){
      this.email = objInit.email;
      this.phoneNumber = objInit.phoneNumber;
   },
   SaleSession: function(objInit) {
      this.tutorPricePerHour = objInit.tutorPricePerHour;
      this.couponCode = objInit.couponCode;
      this.couponType = objInit.couponType;
      this.couponValue = objInit.couponValue;
      this.couponTutor = objInit.couponTutor;
   }
};
function StudyRoomItem(objInit){
   return Object.assign(
      new Item.User(objInit),
      new Item.StudyRoom(objInit)
   );
}
function ContentItem(objInit){
   return Object.assign(
      new Item.Default(objInit),
      new Item[itemTypeChcker(objInit.type)](objInit)
   );
}
function SalesItem(objInit){
   return Object.assign(
      new Item.Default(objInit),
      new Item[itemTypeChcker(objInit.type)](objInit),
      {
         paymentStatus: objInit.paymentStatus,
      }
   );
}
function PurchasesItem(objInit){
   return Object.assign(
      new Item.Default(objInit),
      new Item[itemTypeChcker(objInit.type)](objInit)
   );
}
function FollowersItem(objInit){
   return Object.assign(
      new Item.User(objInit),
      new Item.Follower(objInit)
   )
}
function createSalesItems({data}) {
   let salesItems = [];
   data.forEach(item => salesItems.push(new SalesItem(item)));
   return salesItems;
}
function createSalesSession({data}) {
   return new Item.SaleSession(data);
}
function createContentItems({data}) {
   let contentItems = [];
   data.map(item => contentItems.push(new ContentItem(item)));
   return contentItems;
}
function createPurchasesItems({data}) {
   return data.map(item=> new PurchasesItem(item));
}
function createStudyRoomItems({data}) {
   let studyRoomItems = [];
   data.map(item => studyRoomItems.push(new StudyRoomItem(item)));
   return studyRoomItems;
}
function createFollowersItems({data}) {
   let followersItems = [];
   data.map(item => followersItems.push(new FollowersItem(item)));
   return followersItems;
}

function TutorActions(objInit) {
   this.calendarShared = objInit.calendarShared;
   this.haveHours = objInit.haveHours;
   this.bookedSession = objInit.bookedSession;
}

function createTutorActions({data}) {
   return new TutorActions(data);
}

function createBlogs({data}) {
   return data.map(item => new Blogs.Default(item))
}



function getSalesItems(){
   return connectivityModule.http.get('/Sales/sales').then(createSalesItems).catch(ex => ex);
}
function getSalesSessions(id){
   return connectivityModule.http.get('/Sales/session', {params: { id }}).then(createSalesSession).catch(ex => ex);
}
function updateSessionDuration(session){
   return connectivityModule.http.post('/Sales/duration', session)
}
function getContentItems(){
   return connectivityModule.http.get('/Account/content').then(createContentItems).catch(ex => ex);
}
function getPurchasesItems(){
   return connectivityModule.http.get('/Account/purchases').then(createPurchasesItems).catch(ex => ex);
}
// TODO: move to studyroom service
function getStudyRoomItems(){
   return connectivityModule.http.get('StudyRoom').then(createStudyRoomItems).catch(ex => ex);
}
function getFollowersItems(){
   return connectivityModule.http.get('/Account/followers').then(createFollowersItems).catch(ex => ex);
}
function getTutorActions(){
   return connectivityModule.http.get('/Account/tutorActions').then(createTutorActions).catch(ex => ex);
}
function getSpitballBlogs(){
   return connectivityModule.http.get('/blog').then(createBlogs).catch(ex => ex);
}
function getMarketingBlogs() {
   return connectivityModule.http.get('/blog/marketing').then(createBlogs).catch(ex => ex);
}


export default {
   getSalesItems,
   getSalesSessions,
   updateSessionDuration,
   getContentItems,
   getPurchasesItems,
   getStudyRoomItems,
   getFollowersItems,
   getTutorActions,
   getSpitballBlogs,
   getMarketingBlogs
}
