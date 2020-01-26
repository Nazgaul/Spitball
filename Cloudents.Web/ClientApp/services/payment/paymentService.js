
let country = global.country;

const IL = {
   country: 'IL',
   redeemImg:require('./images/redeemPointsIL.jpg'),
}
const IN = {
   country: 'IN',
   redeemImg:require('./images/redeemPointsFrymo.jpg'),
}
const US = {
   country: 'US',
   redeemImg:require('./images/redeemPointsUS.png'),
}
// redeem func?
// buy points func?
const services = [IL,IN,US]

function getRedeemImg(){
   return services.find(item=>item.country == country).redeemImg
}

export default {
   getRedeemImg
}