const dummyItemObj = {
    id: "3E84FED64DF8FB7B478F5E52DBB470EB",
    snippet: "______________________________________________________________________________________________ ______________________________________________________________________________________________ ____________________________________________________________________________________  __________________________________________________________ ____________________________________________________________________________________",
    source: "_______________________________________________________",
    template: "item",
    skelaton:true,
    title: "________________________________________________",
    url: ""
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
const dummyBookObj = {
    author: "__________________________________________________________________________________________________________",
    binding: "______________________________________________________________________________________",
    edition: "_______________________________________________________",
    isbn10: "______________________________________________________________________________________",
    isbn13: "_______________________________________________________________________________________________",
    template: "book",
    title: "________________________________________________"

};
const dummyJobObj = {
    address: "_____________________",
    company: "_____________________",
    compensationType: "_____________________",
    dateTime: "2018-02-02T00:00:00Z",
    responsibilities: "_______________________________________________________________________________________________________________________________ ____________________________________________________________________________________ ______________________________________________________________________________________________________________________________",
    source: "CareerBuilder",
    template: "job",
    title: "Client Service Representative I",
    url: ""
};

let multipleItemSkeletons = [dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj]
let multipleDummyTutorObj = [dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj]
let multipleDummyBookObj = [dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj]
let multipleDummyJobObj = [dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj]

export const skeletonData = {
    result:multipleItemSkeletons,
    note: multipleItemSkeletons,
    flashcard: multipleItemSkeletons,
    tutor: multipleDummyTutorObj,
    book: multipleDummyBookObj,
    ask: multipleItemSkeletons,
    job: multipleDummyJobObj,
};

export const faqList = [
    { text: 'What is Spitball.co?', href:''},
    { text: 'How does Spitball work?', href:''},
    { text: 'How does Spitball differ from other student websites?', href:''},
    { text: 'Where do the tokens come from?', href:''},
    { text: 'Where does the value of a token come from?', href:''},
    { text: 'What is Vote up and why do we need it?', href:''}
];
export const suggestList={
    job:"Need some cash? Find the best jobs & internships for students at your school.",
    tutor:"Still need help? Connect with a tutor and make sure you ace your class.",
    note:"Searching for study documents for your classes? Look no further.",
    book:"Buying or selling a textbook? We'll make sure you get the best price!",
    ask:"Have a question you need answered? Try us.",
    flashcard:"We found some flashcards that may help you study!"};