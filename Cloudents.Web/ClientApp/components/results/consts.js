const dummyItemObj = {
    id: "3E84FED64DF8FB7B478F5E52DBB470EB",
    snippet: "______________________________________________________________________________________________ ______________________________________________________________________________________________ ____________________________________________________________________________________  __________________________________________________________ ____________________________________________________________________________________",
    source: "_______________________________________________________",
    template: "item",
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
const dummyFoodObj = {
    address: "__________________________________________________________________________________ ___________________________________________________________________________________________",
    hooked: false,
    location: null,
    name: "__________________________________",
    open: true,
    placeId: "ChIJoaWWguy2AhURwxLmvhjuaxI",
    rating: "___________________________________________________",
    template: "food"
};
let multipleItemSkeletons = [dummyItemObj, dummyItemObj, dummyItemObj, dummyItemObj]
export const skeletonData = {
    note: multipleItemSkeletons,
    flashcard: multipleItemSkeletons,
    tutor: [dummyTutorObj, dummyTutorObj, dummyTutorObj, dummyTutorObj],
    book: [dummyBookObj, dummyBookObj, dummyBookObj, dummyBookObj],
    ask: [{ template: 'video-skeleton' }, ...multipleItemSkeletons],
    job: [dummyJobObj, dummyJobObj, dummyJobObj, dummyJobObj],
    food: [dummyFoodObj, dummyFoodObj, dummyFoodObj, dummyFoodObj]
};
export const promotions = {
    note: {
        title: "Study Documents",
        content: "Study documents curated from the best sites on the web filtered by your school, classes and preferences."
    },
    flashcard: {
        title: "Flashcard",
        content: "Search millions of study sets and improve your grades by studying with flashcards."
    },
    ask: {
        title: "Ask A Question",
        content: "Ask any school related question and immediately get answers related specifically to you, your classes, and university."
    },
    tutor: {
        title: "Tutors",
        content: "Spitball has teamed up with the most trusted tutoring services to help you ace your classes."
    },
    book: {
        title: "Textbooks",
        content: "Find the best prices to buy, rent and sell your textbooks by comparing hundreds of sites simultaneously."
    },
    job: {
        title: "Jobs",
        content: "Easily search and apply to paid internships, part-time and entry-level jobs from local businesses to Fortune 500 companies."
    },
    food: {
        title: "Food and Deals",
        content: "Discover exclusive deals to local businesses, restaurants and bars near campus."
    }
};