
let country = global.country;
let siteName = global.siteName;

class CountryPayment {
   constructor(code, img) {
      this.countryCode = code;
      this.redeemImg = img;
   }
   getRedeemImg() {
      return this.redeemImg
   }
}
const IL = new CountryPayment('IL', require('./images/redeemPointsIL.jpg'));
const IN = new CountryPayment('IN', require('./images/redeemPointsFrymo.jpg'));
const US = new CountryPayment('US', require('./images/redeemPointsUS.png'));
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

export default {
   getRedeemImg
}