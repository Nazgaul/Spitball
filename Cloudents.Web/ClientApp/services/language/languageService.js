import { termHtml } from "./page/term"
import { privacyHtml } from "./page/privacy"
import { connectivityModule } from "../connectivity.module";

//global dictionary obj
//global.dictionary = {}

export const LanguageService = {
    getValueByKey: (key) => {
        if(!key) return key;
        
        if(!global.dictionary[key]){
            console.error("dictionary couldnot find key: " + key);
            return `###${key}`;
        }
        return global.dictionary[key];
    },

    getTermPage: ()=>{
        return termHtml;
    },
    getPrivacyPage: ()=>{
        return privacyHtml;
    }     
};

//debug purposes
global.dictionaryFindKey = function(dict, value){
    for(let key in dict){
        if(dict[key] === value){
            console.log(key)
        }
    }
};
global.dictionaryContainsKey = function(dict, value){
    for(let key in dict){
        if(dict[key].indexOf(value) > -1){
            console.log(key)
        }
    }
};