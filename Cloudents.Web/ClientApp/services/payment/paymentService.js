
let country = global.country;
let siteName = global.siteName;

class CountryPayment {
   constructor(code, img,buyPointsComponent,paymentComponent) {
      this.countryCode = code;
      this.redeemImg = img;
      this.buyPointsComponent = buyPointsComponent
      this.paymentComponent = paymentComponent
   }
   getRedeemImg() {
      return this.redeemImg
   }
   getBuyPointsComponent() {
      return this.buyPointsComponent
   }
   getPaymentComponent() {
      return this.paymentComponent
   }
   getIsCalendarNeedPayment(){
      return this.countryCode !== 'US'
   }
}
const IL = new CountryPayment('IL', require('./images/redeemPointsIL.jpg'),'buyPointsIL','paymentIL');
const IN = new CountryPayment('IN', require('./images/redeemPointsFRYMO.jpg'),'buyPointsFrymo');
const US = new CountryPayment('US', require('./images/redeemPointsUS.png'),'buyPointsUS','paymentUS');
const services = [US, IL, IN]


function getTheRightService() {
   let currentService = null;
   services.forEach(countryPayment=>{
      if(countryPayment.countryCode === country){
         currentService = countryPayment;
      }
      if(siteName === 'frymo'){
         currentService = IN;
      }
   })
   return currentService || US;
}

function getRedeemImg(){
   return getTheRightService().getRedeemImg()
}
function getBuyPointsComponent(){
   return getTheRightService().getBuyPointsComponent()
}
function getPaymentComponent(){
   return getTheRightService().getPaymentComponent()
}
function getIsCalendarNeedPayment(){
   return getTheRightService().getIsCalendarNeedPayment()
}

export default {
   getRedeemImg,
   getBuyPointsComponent,
   getPaymentComponent,
   getIsCalendarNeedPayment
}