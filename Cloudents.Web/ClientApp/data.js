import * as routes from "./routeTypes";

export let verticals =
    {
        note: {
            id: routes.notesRoute,
            name: "Study Documents",
            //image: "document",
            // resultTitle: "I found some documents that might help.See what you think!",
            //emptyState: "Darn, I wasn't able to find any documents.Try these other options.",
            filter: [{ id: "course", name: "My Courses" }, { id: "source", name: "sources" }],
            sort: [
                { id: "relevance", name: "relevance" },
                { id: "date", name: "date" }
            ]
        },
        flashcard: {
            id: routes.flashcardRoute,
            name: "Flashcards",
            //image: "flashcard",
            //resultTitle: "Oh look - I found some flashcards for you.Test yourself!",
            //emptyState: "Sorry, I did not find any quiz results...",
            filter: [
                { id: "course", name: "My Courses" },
                { id: "source", name: "sources" }
            ],
            sort: [
                { id: "relevance", name: "relevance" },
                { id: "date", name: "date" }
            ]
        },
        tutor: {
            id: routes.tutorRoute,
            name: "Tutors",
            //image: "tutor",
            //resultTitle: "I found these tutors. Help is just a click away!",
            //emptyState: "Sorry, I did not find any tutors...",
            filter: [
                //{ id: "all", name: "all" },
                { id: "online", name: "Online Lessons" },
                { id: "inPerson", name: "In Person" }
            ],
            sort: [
                { id: "price", name: "price" },
                { id: "distance", name: "distance" }
            ]
        },
        book: {
            id: routes.bookRoute,
            name: "Textbooks"
        },
        ask: {
            id: routes.questionRoute,
            name: "Ask a Question"
            //image: "ask"
            //resultTitle:  "I found a few things that I think are relevant. Check these out.",
            //emptyState: "I could not find any relevant answers.Try these other options.",
            //sort: ""
        },
        job: {
            id: routes.jobRoute,
            name: "Jobs",
            filter: [{ id: "jobType", name: "job type" },{id: "filter", name: "Filter" }],
            sort: [
                { id: "price", name: "price" },
                { id: "distance", name: "distance" }
            ]
        },
        food: {
            id: routes.foodRoute,
            name: "Food and Deals",
            filter: [
                { id: "openNow", name: "Open now" }]
        }

    };
export let details = {
    bookDetails: {
        filter: [ { id: "New", name: "new" }, { id: "Rental", name: "rental" }, { id: "eBook", name: "eBook" }],
        sort: [{ id: "price", name: "price" }]
    }
};
export let actionFunction = {
    ...verticals,
    ...details
};
export let verticalsList = [];
export let names = [];
export let page = [];
export let verticalsNavbar = [];
export let verticalsName=[];

for (var v in verticals) {
    let item = verticals[v];
    verticalsName.push(v);
    names.push({ 'id': item.id, 'name': item.name });
    verticalsNavbar.push(
        {
            'id': item.id,
            'name': item.name
            //image: item.image
        });
    verticalsList.push(verticals[v]);
    page[v] = {
        // title: item.resultTitle,
        //emptyText: item.emptyState,
        filter: item.filter,
        sort: item.sort
    }
}
for (let v in details) {
    let item = details[v];
    page[v] = { filter: item.filter, sort: item.sort }
}

