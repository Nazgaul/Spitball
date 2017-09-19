export let verticals =
    {
        ask :{ name: "ask", image: "ask", placeholder: "Ask anything...", prefix: "" },
        notes :{ name: "notes", image: "document", placeholder: "Find me class materials...", prefix: "" },
        flashcard:{ name: "flashcard", image: "flashcard", placeholder: "Find me flashcards...", prefix: "" },
        tutor:{ name: "tutor", image: "tutor", placeholder: "Find me a tutor...", prefix: "" },
        job:{ name: "job", image: "job", placeholder: "Find me a job...", prefix: "" },
        book:{ name: "book", image: "book", placeholder: "Find me a textbook ...", prefix: "" },
        purchase:{ name: "purchase", image: "purchase", placeholder: "Where can i get...", prefix: "" }
    };

export let verticalsPlaceholders = {};
export let verticalsList=[];
for (var v in verticals) {
    verticalsPlaceholders[v]= verticals[v].placeholder;
}
for (var v in verticals) {
    verticalsList.push(verticals[v]);
}