import {LanguageService} from '../../services/language/languageService'


const dummyItemObj = {
    id: "3E84FED64DF8FB7B478F5E52DBB470EB",
    snippet: "______________________________________________________________________________________________ ______________________________________________________________________________________________ ____________________________________________________________________________________  __________________________________________________________ ____________________________________________________________________________________",
    source: "_______________________________________________________",
    template: "item",
    skelaton:true,
    title: "________________________________________________",
    url: "",
};
const dummyTutorObj = {
    city: "________________________________________",
    description: "_______________________________________________________________________________________ _______________________________________________________________________________________",
    fee: 10,
    location: null,
    name: "________________________________",
    online: false,
    source: "Wyzant",
    state: "",
    template: "tutor",
    url: ""

};


let multipleItemSkeletons = [dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj]
let multipleDummyTutorObj = [dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj]



export const skeletonData = {
    result:multipleItemSkeletons,
    note: multipleItemSkeletons,
    tutor: multipleDummyTutorObj,
    ask: multipleItemSkeletons,
};

// export const faqList = [
//     { text: 'What is Spitball.co?', href:''},
//     { text: 'How does Spitball work?', href:''},
//     { text: 'How does Spitball differ from other student websites?', href:''},
//     { text: 'Where do the tokens come from?', href:''},
//     { text: 'Where does the value of a token come from?', href:''},
//     { text: 'What is Vote up and why do we need it?', href:''}
// ];
export const suggestList={
    tutor:LanguageService.getValueByKey("result_suggestions_tutor"),
    note:LanguageService.getValueByKey("result_suggestions_note"),
    ask:LanguageService.getValueByKey("result_suggestions_ask"),
};