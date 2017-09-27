﻿

export let verticals =
    {
        ask :{ name: "ask", image: "ask", placeholder: "Ask anything...", prefix: "" },
        notes :{ name: "note", image: "document", placeholder: "Find me class materials...", prefix: "" },
        flashcard:{ name: "flashcard", image: "flashcard", placeholder: "Find me flashcards...", prefix: "" },
        tutor:{ name: "tutor", image: "tutor", placeholder: "Find me a tutor...", prefix: "" },
        job:{ name: "job", image: "job", placeholder: "Find me a job...", prefix: "" },
        book:{ name: "book", image: "book", placeholder: "Find me a textbook ...", prefix: "" },
        purchase:{ name: "purchase", image: "purchase", placeholder: "Where can i get...", prefix: "" }
    };

export let verticalsPlaceholders = {};
export let verticalsList=[];
export let names = [];

for (var v in verticals) {
    names.push({ 'id': verticals[v].name, 'name': verticals[v].name });
    verticalsPlaceholders[v] = verticals[v].placeholder;
    verticalsList.push(verticals[v]);
}
