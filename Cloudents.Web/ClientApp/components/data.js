﻿

export let verticals =
    {
        ask: {
            name: "ask", image: "ask", placeholder: "Ask anything...", prefix: "",
            resultTitle: "I’ve come to the conclusion that the answer is:", emptyState: "ask", sort: "", asideLabel:"search type"
        },
        note: {
            name: "note", image: "document", placeholder: "Find me class materials...", prefix: "Find me class material for", asideLabel: "search type",
            resultTitle: "I found some documents that might help.<br>See what you think!", emptyState: "ask", filter: [{ id: 'all', name: 'all' }, { id: 'source', name: 'sources' }], sort: [{ id: 'relevance', name: 'relevance' }, { id: 'date', name: 'date' }]
        },
        flashcard: {
            name: "flashcard", image: "flashcard", placeholder: "Find me flashcards...", prefix: "Find me flashcards on", asideLabel: "search type",
            resultTitle: "Oh look - I found some flashcards for you.<br>Test yourself!", emptyState: "ask", filter: [{ id: 'all', name: 'all' }, { id: 'source', name: 'sources' }], sort: ""
        },
        tutor: {
            name: "tutor", image: "tutor", placeholder: "Find me a tutor...", prefix: "Find me a tutor for", asideLabel: "search type",
            resultTitle: "I found these tutors. Help is juast a click away!", emptyState: "no tutor found...", filter: "Ask anything...", sort: ""
        },
        job: {
            name: "job", image: "job", placeholder: "Find me a job...", prefix: "Find me a job in", asideLabel: "search type",
            resultTitle: "Look at this - you’ll be employed in no time!", emptyState: "ask", filter: "Ask anything...", sort: ""
        },
        book: {
            //TODO: need to check if its the right prefix
            name: "book", image: "book", placeholder: "Find me a textbook ...", prefix: "Textbook by", asideLabel: "search type",
            resultTitle: "I found some textbooks that match your search. Check them out!", emptyState: "ask", filter: "Ask anything...", sort: ""
        },
        purchase: {
            name: "purchase", image: "purchase", placeholder: "Where can i get...", prefix: "Where can I get a", asideLabel: "purchase",
            resultTitle: "Booya! I found a few places that<br>you might like.", emptyState: "ask", filter: "Ask anything...", sort: ""
        }
    };

export let verticalsPlaceholders = {};
export let verticalsList=[];
export let names = [];
export let page = [];
export let prefix = {};

for (var v in verticals) {
    var item = verticals[v];
    names.push({ 'id': item.name, 'name': item.name });
    verticalsPlaceholders[v] = item.placeholder;
    prefix[v] = item.prefix;
    verticalsList.push(verticals[v]);
    page[v] = { title: item.resultTitle, emptyText: item.emptyState,filter:item.filter,sort:item.sort}
}
