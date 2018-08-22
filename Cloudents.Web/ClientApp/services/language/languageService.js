const dictionary = {
    ask: "Ask Your Question",
    faq:"Spitball FAQ",
    more: "More",
    show_me:"Show me",
    spitball:"Spitball",
}


export const LanguageService = {
    getValueByKey: (key) => {
        if(!dictionary[key]){
            return `###${key}`
        }
        return dictionary[key];
    }
}