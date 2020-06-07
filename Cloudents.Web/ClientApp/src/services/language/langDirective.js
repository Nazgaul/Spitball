import { LanguageService } from './languageService'
const swapKeyWithValue = function(el, binding){
    const INNER_HTML = "inner";
        
    //split arguments with delimeter ','
    let arrArgs = binding.arg.split(",");

    //iterate threw all args
    arrArgs.forEach(rawAttr => {
        let attr = rawAttr.trim();
        let keyValue = null;
        let key = null;
        //inner arg will replace the inner HTML
        if(attr === INNER_HTML){
            let fixedKey = binding.value ? binding.value.trim() : el.innerHTML.trim();
            key = LanguageService.getValueByKey(fixedKey);
            el.innerHTML = key;
        }else{
            //other will set the 
            keyValue = binding.value ? binding.value.trim() : el.getAttribute(attr);
            key = LanguageService.getValueByKey(keyValue);
            el.setAttribute(attr, key);
        }
    });
};

export const Language = {
    bind: function(el, binding){
        swapKeyWithValue(el, binding);
    },
    componentUpdated: function(el, binding){
        if(!binding.value){
            // if no value then update wont happen because it will try to put value in the key!
            return;
        }
        swapKeyWithValue(el, binding);
    }
};