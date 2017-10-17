
import * as routes from './../routeTypes';

export let verticals =
    {
        ask: {
            id: routes.questionRoute,
            name: "ask",
            image: "ask",
            placeholder: "Ask anything...",
            prefix: "",
            resultTitle: "I’ve come to the conclusion that the answer is:",
            emptyState: "I could not find any relevant answers. Try these other options.",
            sort: ""
            //asideLabel: "search type"
        },
        note: {
            id: routes.notesRoute,

            name: "note", image: "document", placeholder: "Find me class materials...",
            prefix: "Find me class material for",
            //asideLabel: "search type",
            resultTitle: "I found some documents that might help.<br>See what you think!",
            emptyState: "Darn, U wasn't able to find any documents. Try these other options.",
            filter: [{ id: 'all', name: 'all' },
                { id: 'source', name: 'sources' }], sort: [{ id: 'relevance', name: 'relevance' },
                { id: 'date', name: 'date' }]
        },
        flashcard: {
            id: routes.flashcardRoute,

            name: "flashcard", image: "flashcard", placeholder: "Find me flashcards...",
            prefix: "Find me flashcards on",
            //asideLabel: "search type",
            resultTitle: "Oh look - I found some flashcards for you.<br>Test yourself!",
            emptyState: "Sorry, I did not find any quiz results...",
            filter: [{ id: 'all', name: 'all' }, { id: 'source', name: 'sources' }],
            sort: [{ id: 'relevance', name: 'relevance' }, { id: 'date', name: 'date' }]
        },
        tutor: {
            id: routes.tutorRoute,

            name: "tutor", image: "tutor", placeholder: "Find me a tutor...",
            prefix: "Find me a tutor for",
            //asideLabel: "search type",
            resultTitle: "I found these tutors. Help is juast a click away!",
            emptyState: "Sorry, I did not find any tutors...",
            filter: [{ id: 'all', name: 'all' }, { id: 'online', name: 'online' }, { id: 'inPerson', name: 'In Person' }],
            sort: [{ id: 'relevance', name: 'relevance' }, { id: 'price', name: 'price' }, { id: 'distance', name: 'distance' }]
        },
        job: {
            id: routes.jobRoute,

            name: "job",
            image: "job",
            placeholder: "Find me a job...",
            prefix: "Find me a job in",
            resultTitle: "Look at this - you’ll be employed in no time!",
            emptyState: "Sorry, I did not find any jobs that match your search...",
            filter: [{ id: 'all', name: 'all' },{ id: 'jobType', name: 'job type' }, { id: 'paid', name: 'paid' }],
            sort: [{ id: 'relevance', name: 'relevance' }, { id: 'price', name: 'price' }, { id: 'distance', name: 'distance' }]
        },
        book: {
            id: routes.bookRoute,

            name: "book", image: "book", placeholder: "Find me a textbook ...", prefix: "Find me a textbook -",
            //asideLabel: "search type",
            resultTitle: "I found some textbooks that match your search. Check them out!", emptyState: "Sorry, I did not find any textbooks that match your search...", filter: "", sort: ""
        },
        food: {
            id: routes.foodRoute,

            name: "food",
            image: "food",
            placeholder: "Where can i get...",
            prefix: "Where can I get a",
            //asideLabel: "purchase",
            resultTitle: "Booya! I found a few places that<br>you might like.", emptyState: "Sorry, friend! I couldn't find anywhere nearby that sells $subject", filter: [{ id: 'all', name: 'all' }, { id: 'openNow', name: 'open now' }], sort: [{ id: 'distance', name: 'distance' }]
        }
       
    };
export let details = {
    bookDetails: {
        filter: [{ id: 'all', name: 'all' }, { id: 'new', name: 'new' }, { id: 'rental', name: 'rental' }, { id: 'eBook', name: 'eBook' }],
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
export let prefix = {};

export let verticalsNavbar = [
    {
        label: "search type",
        data: [
            {
                id: verticals[routes.questionRoute].id,
                name: verticals[routes.questionRoute].name,
                image: verticals[routes.questionRoute].image
            },
            {
                id: verticals[routes.notesRoute].name,
                name: verticals[routes.notesRoute].name,
                image: verticals[routes.notesRoute].image
            },
            {
                id: verticals[routes.flashcardRoute].name,
                name: verticals[routes.flashcardRoute].name,
                image: verticals[routes.flashcardRoute].image
            },
            {
                id: verticals[routes.tutorRoute].name,
                name: verticals[routes.tutorRoute].name,
                image: verticals[routes.tutorRoute].image
            },
            { id: verticals[routes.jobRoute].name, name: verticals[routes.jobRoute].name, image: verticals[routes.jobRoute].image },
            { id: verticals[routes.bookRoute].name, name: verticals[routes.bookRoute].name, image: verticals[routes.bookRoute].image }
        ]
    },
    {
        label: "purchase",
        data: [
            {
                id: verticals[routes.foodRoute].id,
                name: verticals[routes.foodRoute].name,
                image: verticals[routes.foodRoute].image
            }
        ]
    },
    {
        label: "Contribute",
        data: [
            {
                id: routes.uploadRoute,
                name: "upload",
                image: "upload"
            },
            {
                id: routes.postRoute,
                name: "post",
                image: "post"
            },
            {
                id: routes.createFlashcard,
                name: "Create Flashcard",
                image: "flashcard"
            }
        ]
    },
    {
        label: "Class talk",
        data: [
            {
                id: routes.chatRoute,
                name: "chat",
                image: "chat"
            }
        ]
    },
    {
        label: "Personal info",
        data: [
            {
                id: routes.coursesRoute,
                name: "My Courses",
                image: "courses"
            },
            {
                id: routes.likesRoute,
                name: "My likes",
                image: "likes"
            },
            {
                id: routes.settingsRoute,
                name: "Settings",
                image: "courses" //missing
            }
        ]
    },
];

for (var v in verticals) {
    var item = verticals[v];
    names.push({ 'id': item.name, 'name': item.name });
    verticalsPlaceholders[v] = item.placeholder;
    prefix[v] = item.prefix;
    verticalsList.push(verticals[v]);
    page[v] = { title: item.resultTitle, emptyText: item.emptyState,filter:item.filter,sort:item.sort}
}
for (var v in details) {
    var item = details[v]
    page[v] = {filter: item.filter, sort: item.sort }
}

