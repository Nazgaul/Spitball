

export let verticals =
    {
        ask: {
            name: "ask", image: "ask", placeholder: "Ask anything...", prefix: "",
            resultTitle: "I’ve come to the conclusion that the answer is:", emptyState: "ask", sort: "", asideLabel:"search type"
        },
        note: {
            name: "note", image: "document", placeholder: "Find me class materials...", prefix: "", asideLabel: "search type",
            resultTitle: "I found some documents that might help.<br>See what you think!", emptyState: "ask", filter: [{ id: 'all', name: 'all' }, { id: 'sources', name: 'sources' }], sort: [{ id: 'relevance', name: 'relevance' }, { id: 'name', name: 'name' }]
        },
        flashcard: {
            name: "flashcard", image: "flashcard", placeholder: "Find me flashcards...", prefix: "", asideLabel: "search type",
            resultTitle: "Oh look - I found some flashcards for you.<br>Test yourself!", emptyState: "ask", filter: [{ id: 'all', name: 'all' }, { id: 'sources', name: 'sources' }], sort: ""
        },
        tutor: {
            name: "tutor", image: "tutor", placeholder: "Find me a tutor...", prefix: "", asideLabel: "search type",
            resultTitle: "I found these tutors. Help is juast a click away!", emptyState: "no tutor found...", filter: "Ask anything...", sort: ""
        },
        job: {
            name: "job", image: "job", placeholder: "Find me a job...", prefix: "", asideLabel: "search type",
            resultTitle: "Look at this - you’ll be employed in no time!", emptyState: "ask", filter: "Ask anything...", sort: ""
        },
        book: {
            name: "book", image: "book", placeholder: "Find me a textbook ...", prefix: "", asideLabel: "search type",
            resultTitle: "I found some textbooks that match your search. Check them out!", emptyState: "ask", filter: "Ask anything...", sort: ""
        },
        purchase: {
            name: "purchase", image: "purchase", placeholder: "Where can i get...", prefix: "", asideLabel: "purchase",
            resultTitle: "Booya! I found a few places that<br>you might like.", emptyState: "ask", filter: "Ask anything...", sort: ""
        }
    };

export let verticalsPlaceholders = {};
export let verticalsList=[];
export let names = [];
export let page = [];

for (var v in verticals) {
    var item = verticals[v];
    names.push({ 'id': item.name, 'name': item.name });
    verticalsPlaceholders[v] = item.placeholder;
    verticalsList.push(verticals[v]);
    page[v] = { title: item.resultTitle, emptyText: item.emptyState,filter:item.filter,sort:item.sort}
}
