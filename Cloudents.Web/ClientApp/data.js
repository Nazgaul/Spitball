import * as routes from "./routeTypes";

export let verticals =
    {
        note: {
            id: routes.notesRoute,
            name: "Study Documents",
            needLocation:false,
            filter: [{ id: "course", name: "My Courses" }, { id: "source", name: "sources" }],
            sort: [
                { id: "relevance", name: "relevance" },
                { id: "date", name: "date" }
            ]
        },
        flashcard: {
            id: routes.flashcardRoute,
            name: "Flashcards",
            needLocation: false,
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
            needLocation: true,
            filter: [
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
            filter:[{ id: "source", name: "sources" }],
            id: routes.questionRoute,
            name: "Ask a Question"
            
        },
        job: {
            id: routes.jobRoute,
            name: "Jobs",
            needLocation: true,
            filter: [{ id: "jobType", name: "job type" }],
            sort: [
                { id: "distance", name: "distance" },
                { id: "date", name: "date" }
            ]
        },
        food: {
            id: routes.foodRoute,
            needLocation: true,
            name: "Food and Deals",
            filter: [
                { id: "openNow", name: "Open now" }]
        }

    };
export let details = {
    bookDetails: {
        filter: [ { id: "New", name: "new" }, { id: "Rental", name: "rental" }, { id: "eBook", name: "eBook" },{ id: "Used", name: "used" }],
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

for (let item of verticals) {
    // let item = verticals[v];
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

