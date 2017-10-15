
import * as routes from './../routeTypes';

export let verticals =
    {
        ask: {
            name: "ask", image: "ask", placeholder: "Ask anything...", prefix: "",
            resultTitle: "I’ve come to the conclusion that the answer is:", emptyState: "I couldnt found results for your questions..", sort: "", asideLabel:"search type"
        },
        note: {
            name: "note", image: "document", placeholder: "Find me class materials...", prefix: "Find me class material for", asideLabel: "search type",
            resultTitle: "I found some documents that might help.<br>See what you think!", emptyState: "no document founds", filter: [{ id: 'all', name: 'all' }, { id: 'source', name: 'sources' }], sort: [{ id: 'relevance', name: 'relevance' }, { id: 'date', name: 'date' }]
        },
        flashcard: {
            name: "flashcard", image: "flashcard", placeholder: "Find me flashcards...", prefix: "Find me flashcards on", asideLabel: "search type",
            resultTitle: "Oh look - I found some flashcards for you.<br>Test yourself!", emptyState: "no flashcards found", filter: [{ id: 'all', name: 'all' }, { id: 'source', name: 'sources' }], sort: [{ id: 'relevance', name: 'relevance' }, { id: 'date', name: 'date' }]
        },
        tutor: {
            name: "tutor", image: "tutor", placeholder: "Find me a tutor...", prefix: "Find me a tutor for", asideLabel: "search type",
            resultTitle: "I found these tutors. Help is juast a click away!", emptyState: "no tutor found...", filter: [{ id: 'all', name: 'all' }, { id: 'online', name: 'online' }, { id: 'inPerson', name: 'In Person' }], sort: [{ id: 'relevance', name: 'relevance' }, { id: 'price', name: 'price' }, { id: 'distance', name: 'distance' }]
        },
        job: {
            name: "job", image: "job", placeholder: "Find me a job...", prefix: "Find me a job in", asideLabel: "search type",
            resultTitle: "Look at this - you’ll be employed in no time!", emptyState: "no job founds", filter: [{ id: 'all', name: 'all' }, { id: 'jobType', name: 'job type' }, { id: 'paid', name: 'paid' }], sort:[{ id: 'relevance', name: 'relevance' }, { id: 'price', name: 'price' }, { id: 'distance', name: 'distance' }]
        },
        book: {
            name: "book", image: "book", placeholder: "Find me a textbook ...", prefix: "Find me a textbook -", asideLabel: "search type",
            resultTitle: "I found some textbooks that match your search. Check them out!", emptyState: "no Books founds", filter: "", sort: ""
        },
        food: {
            name: "food", image: "food", placeholder: "Where can i get...", prefix: "Where can I get a", asideLabel: "food",
            resultTitle: "Booya! I found a few places that<br>you might like.", emptyState: "no Food founds", filter: [{ id: 'all', name: 'all' }, { id: 'openNow', name: 'open now' }], sort: [{ id: 'distance', name: 'distance' }]
        }
    };

export let verticalsPlaceholders = {};
export let verticalsList=[];
export let names = [];
export let page = [];
export let prefix = {};

export let verticalsNavbar = [{
    label: "search type", data: [{ id: routes.questionRoute, name: verticals[routes.questionRoute].name, image: verticals[routes.questionRoute].image },
        { id: routes.notesRoute, name: verticals[routes.notesRoute].name, image: verticals[routes.notesRoute].image },
        { id: routes.flashcardRoute, name: verticals[routes.flashcardRoute].name, image: verticals[routes.flashcardRoute].image },
        { id: routes.tutorRoute, name: verticals[routes.tutorRoute].name, image: verticals[routes.tutorRoute].image },
        { id: routes.jobRoute, name: verticals[routes.jobRoute].name, image: verticals[routes.jobRoute].image },
        { id: routes.bookRoute, name: verticals[routes.bookRoute].name, image: verticals[routes.bookRoute].image }
    ]
},
    {
        label: "food", data: [
            { id: routes.foodRoute, name: verticals[routes.foodRoute].name, image: verticals[routes.foodRoute].image }

        ]
    }
]

for (var v in verticals) {
    var item = verticals[v];
    names.push({ 'id': item.name, 'name': item.name });
    verticalsPlaceholders[v] = item.placeholder;
    prefix[v] = item.prefix;
    verticalsList.push(verticals[v]);
    page[v] = { title: item.resultTitle, emptyText: item.emptyState,filter:item.filter,sort:item.sort}
}
