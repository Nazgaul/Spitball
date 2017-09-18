//import vertical from './vertical.vue';
import ask from './images/ask.svg';
import book from './images/book.svg';
import document from './images/document.svg';
import flashcard from './images/flashcard.svg';
import job from './images/job.svg';
import purchase from './images/purchase.svg';
import tutor from './images/tutor.svg';




//let selected = verticals[0];

export default {

    name: 'vertical-collection',
    components: {
        ask, book, document, flashcard, job, purchase, tutor
    },
    props: {
        changeCallback: {type:Function}
    },
    data() {
        const verticals =
        [
            { name: "ask", image: "ask" , placeholder: "Ask anything...", prefix: ""},
            { name: "notes", image: "document", placeholder: "Find me class materials...", prefix: ""},
            { name: "flashcard", image: "flashcard", placeholder: "Find me flashcards...", prefix: "" },
            { name: "tutor", image: "tutor", placeholder: "Find me a tutor...", prefix: "" },
            { name: "job", image: "job", placeholder: "Find me a job...", prefix: "" },
            { name: "book", image: "book", placeholder: "Find me a textbook ...", prefix: ""},
            { name: "purchase", image: "purchase", placeholder: "Where can i get...", prefix: "" }
        ];
        return {
            verticals : verticals,
            selected: verticals[0]
        }
    },
    methods: {
        change(vertical) {
           this.selected = vertical;
           this.changeCallback(vertical);
        }
    }
};