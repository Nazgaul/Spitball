import Vue from 'vue';
import { router } from '../main.js';
import { LanguageService } from '../services/language/languageService';
import utilitiesService from '../services/utilities/utilitiesService';


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
Vue.prototype.$Ph = function (key, placeholders) {
    let rawKey = LanguageService.getValueByKey(key);
    //if no placeholders return the Key without the replace
    if (!placeholders) {
        //console.error(`${key} have no placeholders to replace`)
        return rawKey;
    }

    let argumentsToSend = [];
    //placeholders must be an array
    if (Array.isArray(placeholders)) {
        argumentsToSend = placeholders;
    } else {
        argumentsToSend = [placeholders];
    }
    return LanguageService.changePlaceHolders(rawKey, argumentsToSend);
};
Vue.prototype.$chatMessage = function (message) {
    let userName = this.$store.getters.accountUser.id == message.userId ? '' : `<span style="font-weight: 600;">${message.name}:</span><br>`
    if(message.type === 'text'){
        //text and convert links to url's
        let linkTest = /(ftp:\/\/|www\.|https?:\/\/){1}[a-zA-Z0-9u00a1-\\uffff0-]{2,}\.[a-zA-Z0-9u00a1-\\uffff0-]{2,}(\S*)/g;
        let modifiedText = message.text;
        let matchedResults = modifiedText.match(linkTest);

        if(!!matchedResults){
            matchedResults.forEach(result=>{
                let prefix = result.toLowerCase().indexOf('http') === -1 &&
                result.toLowerCase().indexOf('ftp') === -1 ? '//' : '';
                modifiedText = modifiedText.replace(result, `<a href="${prefix}${result}" target="_blank">${result}</a>`);
            });
        } 
        return userName + modifiedText;
    }else{
        let src = utilitiesService.proccessImageURL(message.src, 190, 140, 'crop');
        let modifiedMessage = `<a href="${message.href}" target="_blank"><img src="${src}"/></a>`;
        return userName + modifiedMessage
    }
};