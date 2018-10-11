import { termHtml } from "./page/term"
import { privacyHtml } from "./page/privacy"
import { connectivityModule } from "../connectivity.module";

//global dictionary obj
//global.dictionary = {}
var locale = {};

const LanguageService = {
    getValueByKey: (key) => {
        if(!key) return key;

        if(!locale[key]){
            console.error("dictionary couldnot find key: " + key);
            return `###${key}`;
        }
        return locale[key];
    },

    getTermPage: ()=>{
        return termHtml;
    },
    getPrivacyPage: ()=>{
        return privacyHtml;
    }     
};

const LanguageChange = {
    setUserLanguage: (locale) => {
        return connectivityModule.http.post("/Account/language", {culture: locale})
    },
};

const GetDictionary = () => {
    return connectivityModule.http.get("/Locale").then((dictionary)=>{
        locale = dictionary.data;
        return Promise.resolve(true);
    }, (err)=>{
        return Promise.reject(err);
    })
}

    


//debug purposes
global.dictionaryFindKey = function(value){
    for(let key in locale){
        if(locale[key] === value){
            console.log(key)
        }
    }
};
global.dictionaryContainsKey = function(value){
    for(let key in locale){
        if(locale[key].indexOf(value) > -1){
            console.log(key)
        }
    }
};

export{
    LanguageService,
    LanguageChange,
    GetDictionary
}