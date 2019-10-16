import { connectivityModule } from "../connectivity.module";

//global dictionary obj
//global.dictionary = {}
var locale = {};

const LanguageService = {
    getValueByKey: (key) => {
        if(!key) return key;

        if(!locale.hasOwnProperty(key)){
            console.error("dictionary couldnot find key: " + key);
            return `###${key}`;
        }
        return locale[key];
    },
    changePlaceHolders: (rawKey, arrPlaceholders)=>{
        let key = rawKey;
        arrPlaceholders.forEach((placeHolder, index)=>{
            //placeholder numbers starts with 1
            let placeHolderNum = index + 1;
            key = key.replace(`{${placeHolderNum}}`, placeHolder);
        });
        return key;
    }    
};

const LanguageChange = {
    setUserLanguage: (locale) => {
        return connectivityModule.http.post("/Account/language", {culture: locale});
    },
};

const GetDictionary = (type) => {
    let dictionaryType = `?v=${global.version}&l=${global.lang}`;
    if(!!type){
        //version is for anti caching ability
        dictionaryType += `&resource=${type}`;
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
    });
};

const GetVersion = () => {
    return connectivityModule.http.get(`/homepage/version`).then( (response) =>{
        let version = response.data.version;
        return Promise.resolve(version);
    }, (err)=>{
        return Promise.reject(err);
    });
};

//debug purposes
global.dictionaryFindKey = function(value){
    for(let key in locale){
        if(locale[key] === value){
            console.log(key);
        }
    }
};
global.dictionaryContainsKey = function(value){
    for(let key in locale){
        if(locale[key].indexOf(value) > -1){
            console.log(key);
        }
    }
};

export{
    LanguageService,
    LanguageChange,
    GetDictionary,
    GetVersion
}