import { termHtml } from "./page/term"
import { connectivityModule } from "../connectivity.module";

//global dictionary obj
//global.dictionary = {}

export const LanguageService = {
    getDictionary: () => {
        connectivityModule.http.get("/language").then(function(languageJson) {
            global.dictionary = languageJson.data;
        })
    },
    getValueByKey: (key) => {
        if(!global.dictionary[key]){
            return `###${key}`;
        }
        return global.dictionary[key];
    },

    getTermPage: ()=>{
        return termHtml;
    }
};