import * as routes from "./routeTypes";

export let verticals =
    {

        // marketplace: {
        //     id: routes.marketRoute,
        //     name: "Marketplace",
        //     icon: "sbf-marketplace"
        //
        // },
        ask: {
            filter:[{ id: "source", name: "subject" }],
            id: routes.questionRoute,
            name: "Ask a Question",
            icon: "sbf-ask-q"

        },
        note: {
            id: routes.notesRoute,
            name: "Study Documents",
            needLocation:false,
            filter: [{ id: "course", name: "My Courses" }, { id: "source", name: "sources" }],
            sort: [
                { id: "relevance", name: "relevance" },
                { id: "date", name: "date" }
            ],
            icon: "sbf-note"
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
            ],
            icon: "sbf-flashcards"
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
                { id: "relevance", name: "relevance" },
                { id: "price", name: "price" }
                // { id: "distance", name: "distance" }
            ],
            icon: "sbf-tutor"
        },
        book: {
            id: routes.bookRoute,
            name: "Textbooks",
            icon: "sbf-textbooks"
        },
        job: {
            id: routes.jobRoute,
            name: "Jobs",
            needLocation: true,
            filter: [{ id: "jobType", name: "job type" }],
            sort: [
                { id: "relevance", name: "relevance" },
                // { id: "distance", name: "distance" },
                { id: "date", name: "date" }
            ],
            icon: "sbf-job"
        }
    };
export let details = {
    bookDetails: {
        filter: [ { id: "new", name: "new" }, { id: "rental", name: "rental" }, { id: "eBook", name: "eBook" },{ id: "used", name: "used" }],
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

for (let v in verticals) {
    let item = verticals[v];
    verticalsName.push(v);
    names.push({ 'id': item.id, 'name': item.name });
    verticalsNavbar.push(
        {
            'id': item.id,
            'name': item.name,
            'icon': item.icon
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

