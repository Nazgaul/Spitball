//import vertical from './vertical.vue';
import ask from './images/ask.svg';
import book from './images/book.svg';
import document from './images/document.svg';
import flashcard from './images/flashcard.svg';
import job from './images/job.svg';
import food from './images/food.svg';
import tutor from './images/tutor.svg';
import { verticalsList as verticals } from '../data.js'




//let selected = verticals[0];

export default {
    components: {
        ask, book, document, flashcard, job, food, tutor
    },
    props: {
        changeCallback: {type:Function}
    },
    data() {
        return {
            verticals : verticals,
            selected: this.$route.meta.searchType.length ? this.$route.meta.searchType : verticals[0].name
        }
    },
    //computed: {
    //    selected: () => { return this.$router.meta.searchType ? this.$router.meta.searchType:verticals[0]}
    //},
    methods: {
        change(vertical) {
           this.selected = vertical.name;
           this.changeCallback(vertical);
        }
    }
};