import {dictionary} from "./languageDictionary"
import { termHtml } from "./page/term"

export const LanguageService = {
    getValueByKey: (key) => {
        if(!dictionary[key]){
            return `###${key}`;
        }
        return dictionary[key];
    },

    getTermPage: ()=>{
        return termHtml;
    }
};