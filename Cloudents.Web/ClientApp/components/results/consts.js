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
export const skeletonData = {
    result:multipleItemSkeletons,
    note: multipleItemSkeletons,
    flashcard: multipleItemSkeletons,
    tutor: [dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj],
    book: [dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj],
    ask: multipleItemSkeletons,
    job: [dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj],
};
// export const promotions = {
//     note: {
//         title: "Study Documents",
//         content: "Study documents curated from the best sites on the web filtered by your school, classes and preferences."
//     },
//     flashcard: {
//         title: "Flashcards",
//         content: "Search millions of study sets and improve your grades by studying with flashcards."
//     },
//     ask: {
//         title: "Ask A Question",
//         content: "Ask any school related question and immediately get answers related specifically to you, your classes, and university."
//     },
//     tutor: {
//         title: "Tutors",
//         content: "Spitball has teamed up with the most trusted tutoring services to help you ace your classes."
//     },
//     book: {
//         title: "Textbooks",
//         content: "Find the best prices to buy, rent and sell your textbooks by comparing hundreds of sites simultaneously."
//     },
//     job: {
//         title: "Jobs",
//         content: "Easily search and apply to paid internships, part-time and entry-level jobs from local businesses to Fortune 500 companies."
//     }
//
// };



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