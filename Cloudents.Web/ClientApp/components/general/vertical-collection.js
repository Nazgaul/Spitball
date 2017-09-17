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
    data() {
        const verticals =
        [
            { name: "ask", image: "ask" },
            { name: "notes", image: "document" },
            { name: "flashcard", image: "flashcard" },
            { name: "tutor", image: "tutor" },
            { name: "job", image: "job" },
            { name: "book", image: "book" },
            { name: "purchase", image: "purchase" }
        ];
        return {
            verticals : verticals,
            selected: verticals[0]
        }
    },
    methods: {
        change(vertical) {
           this.counter += 1;
           this.selected = vertical;
        }
    }
};