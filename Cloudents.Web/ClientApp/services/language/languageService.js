import {dictionary} from "./languageDictionary"



export const LanguageService = {
    getValueByKey: (key) => {
        if(!dictionary[key]){
            return `###${key}`
        }
        return dictionary[key];
    }
}