const dictionary = {
    key: "value from dictionary",
    s: "value from dictionary",
    
}

export const LanguageService = {
    getValueByKey: (key) => {
        if(!dictionary[key]){
            return `###${key}`
        }
        return dictionary[key];
    }
}