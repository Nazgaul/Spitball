import Vue from 'vue';
import { router } from '../main.js';
import { i18n } from '../plugins/t-i18n'

Vue.prototype.$openDialog = function(dialogName, params){
    router.push({query:{...router.currentRoute.query,dialog:dialogName}, params: {...params}}).catch(()=>{})
}
Vue.prototype.$closeDialog = function(){
    router.push({query:{...router.currentRoute.query,dialog:undefined}}).catch(()=>{})
}
Vue.prototype.$loadStyle = function(url,id){
    return new Promise((resolve) => {
        if (document.querySelector(id)) return resolve();
        let linkTag = document.createElement('link');
        linkTag.id = id;
        linkTag.rel = 'stylesheet';
        linkTag.href = url;
        document.head.insertBefore(linkTag, document.head.firstChild);
        return resolve();
    });
};
Vue.prototype.$proccessImageUrl = function(url, width, height, mode){
    let usedMode = mode ? mode : 'crop';
    if(url){
        let returnedUrl = `${url}?&width=${width}&height=${height}&mode=${usedMode}`;
        return returnedUrl;
    }else{
        return '';
    }
};
// Vue.prototype.$chatMessage = function (message) {
//     let userName = this.$store.getters.accountUser.id == message.userId ? '' : `<span style="font-weight: 600;display: block;margin-bottom: 6px;">${message.name}:</span>`
//     if(message.type === 'text'){
//         //text and convert links to url's
//         let linkTest = /(ftp:\/\/|www\.|https?:\/\/){1}[a-zA-Z0-9u00a1-\\uffff0-]{2,}\.[a-zA-Z0-9u00a1-\\uffff0-]{2,}(\S*)/g;
//         let modifiedText = message.text;
//         let matchedResults = modifiedText.match(linkTest);

//         if(!!matchedResults){
//             matchedResults.forEach(result=>{
//                 let prefix = result.toLowerCase().indexOf('http') === -1 &&
//                 result.toLowerCase().indexOf('ftp') === -1 ? '//' : '';
//                 modifiedText = modifiedText.replace(result, `<a href="${prefix}${result}" target="_blank">${result}</a>`);
//             });
//         } 
//         return userName + modifiedText;
//     }else{
//         let src = utilitiesService.proccessImageURL(message.src, 190, 140, 'crop');
//         let modifiedMessage = `<a href="${message.href}" target="_blank"><img src="${src}"/></a>`;
//         return userName + modifiedMessage
//     }
// };

Vue.prototype.$price = function(price, currency,freeText = false, minFraction = 0, maxFraction = 0) {
    if (price instanceof Object) {
        currency = price.currency;
        price = price.price;
    }
    if (freeText && price === 0) {
        return i18n.t('free');
    }
    let options = {
        style: 'currency',
        currency: currency,
        minimumFractionDigits: minFraction,
        maximumFractionDigits: maxFraction
    }
    return i18n.n(price, options)
}
Date.prototype.FormatDateToString = function() {
    //https://stackoverflow.com/questions/23593052/format-javascript-date-as-yyyy-mm-dd
    var d = new Date(this),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) 
        month = '0' + month;
    if (day.length < 2) 
        day = '0' + day;

    return [year, month, day].join('-');
}

Date.prototype.AddDays = function(days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
}