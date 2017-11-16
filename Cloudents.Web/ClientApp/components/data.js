
import * as routes from './../routeTypes';

export let verticals =
    {
        ask: {
            id: routes.questionRoute,
            name: "ask",
            image: "ask",
            placeholder: "Ask anything...",
            resultTitle: { short: "I’ve come to the conclusion that the answer is:", normal: "I found a few things that I think are relevant. Check these out."},
            emptyState: "I could not find any relevant answers.Try these other options.",
            sort: ""
        },
        note: {
            id: routes.notesRoute,

            name: "Class Material", image: "document", placeholder: "Find me class material...",
            resultTitle: "I found some documents that might help.See what you think!",
            emptyState: "Darn, I wasn't able to find any documents.Try these other options.",
            filter: [{ id: 'all', name: 'all' },
                { id: 'source', name: 'sources' }, {id:'course',name:"My Courses"}], sort: [{ id: 'relevance', name: 'relevance' },
                { id: 'date', name: 'date' }]
        },
        flashcard: {
            id: routes.flashcardRoute,

            name: "Flashcards", image: "flashcard", placeholder: "Find me flashcards...",
            resultTitle: "Oh look - I found some flashcards for you.Test yourself!",
            emptyState: "Sorry, I did not find any quiz results...",
            filter: [{ id: 'all', name: 'all' }, { id: 'source', name: 'sources' }],
            sort: [{ id: 'relevance', name: 'relevance' }, { id: 'date', name: 'date' }]
        },
        tutor: {
            id: routes.tutorRoute,

            name: "Tutors", image: "tutor", placeholder: "Find me a tutor...",
            resultTitle: "I found these tutors. Help is just a click away!",
            emptyState: "Sorry, I did not find any tutors...",
            filter: [{ id: 'all', name: 'all' }, { id: 'online', name: 'online' }, { id: 'inPerson', name: 'In Person' }],
            sort: [{ id: 'relevance', name: 'relevance' }, { id: 'price', name: 'price' }, { id: 'distance', name: 'distance' }]
        },
        job: {
            id: routes.jobRoute,

            name: "Jobs",
            image: "job",
            placeholder: "Find me a job...",
            resultTitle: "Look at this - you’ll be employed in no time!",
            emptyState: "Sorry, I didn't find any jobs that match your search",
            filter: [{ id: 'all', name: 'all' },{ id: 'jobType', name: 'job type' }, { id: 'paid', name: 'paid' }],
            sort: [{ id: 'relevance', name: 'relevance' }, { id: 'price', name: 'price' }, { id: 'distance', name: 'distance' }]
        },
        book: {
            id: routes.bookRoute,

            name: "Textbooks", image: "book", placeholder: "Find me a textbook...",
            resultTitle: "I found some textbooks that match your search. Check them out!", emptyState: "Sorry, I didn't find any textbooks that match your search", filter: "", sort: ""
        },
        food: {
            id: routes.foodRoute,

            name: "Food",
            image: "food",
            placeholder: "Where can I get...",
            resultTitle: "Booya! I found a few places that.you might like.", emptyState: "Sorry, friend! I could not find anywhere nearby that sells $subject", filter: [{ id: 'all', name: 'all' }, { id: 'openNow', name: 'open now' }], sort: [{ id: 'distance', name: 'distance' }]
        }
       
    };
export let details = {
    bookDetails: {
        filter: [{ id: 'all', name: 'all' }, { id: 'New', name: 'new' }, { id: 'Rental', name: 'rental' }, { id: 'eBook', name: 'eBook' }],
        sort: [{ id: 'price', name: 'price' }]
    }
}
export let actionFunction = {
        ...verticals,
        ...details
};
export let verticalsPlaceholders = {};
export let verticalsList=[];
export let names = [];
export let page = [];
export let verticalsNavbar=[];

for (var v in verticals) {
    var item = verticals[v];
    names.push({ 'id': item.id, 'name': item.name });
    verticalsNavbar.push({'id':item.id,'name':item.name,image:item.image});
    verticalsPlaceholders[v] = item.placeholder;
    verticalsList.push(verticals[v]);
    page[v] = { title: item.resultTitle, emptyText: item.emptyState,filter:item.filter,sort:item.sort}
}
verticalsNavbar.push({
    id: routes.settingsRoute,
    name: "Settings",
    image: "setting"
});
for (var v in details) {
    var item = details[v]
    page[v] = {filter: item.filter, sort: item.sort }
}

