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
    }    
};

const LanguageChange = {
    setUserLanguage: (locale) => {
        return connectivityModule.http.post("/Account/language", {culture: locale})
    },
};

const GetDictionary = (type) => {
    let dictionaryType = `?v=${global.version}&l=${global.lang}`;
    if(!!type){
        //version is for anti caching ability
        dictionaryType += `&resource=${type}`
    }else{
        dictionaryType += '';
    }
    return connectivityModule.http.get(`/Locale${dictionaryType}`).then((dictionary)=>{
        for(let prop in dictionary.data){
            //add the key to the dictionary
            locale[prop] = dictionary.data[prop];
        }
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