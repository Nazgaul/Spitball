
let country = global.country;
let siteName = global.siteName;

class CountryPayment {
   constructor(code, img,buyPointsComponent) {
      this.countryCode = code;
      this.redeemImg = img;
      this.buyPointsComponent = buyPointsComponent
   }
   getRedeemImg() {
      return this.redeemImg
   }
   getBuyPointsComponent() {
      return this.buyPointsComponent
   }
}
const IL = new CountryPayment('IL', require('./images/redeemPointsIL.jpg'),'buyPoints');
const IN = new CountryPayment('IN', require('./images/redeemPointsFRYMO.jpg'),'buyPointsFrymo');
const US = new CountryPayment('US', require('./images/redeemPointsUS.png'),'buyPointsUS');
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

export default {
   getRedeemImg,
   getBuyPointsComponent
}