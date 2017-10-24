import askHeader from '../navbar/images/ask.svg'
import flashcardHeader from '../navbar/images/flashcard.svg'
import jobHeader from '../navbar/images/job.svg'
import bookHeader from '../navbar/images/book.svg'
import noteHeader from '../navbar/images/document.svg'
import tutorHeader from '../navbar/images/tutor.svg'
import foodHeader from '../navbar/images/food.svg'
import searchTypes from './../helpers/radioList.vue'
import searchIcon from './Images/search-icon.svg';
import { mapActions} from 'vuex'
import { verticalsPlaceholders as placeholders, names } from '../data'
import logo from '../../../wwwroot/Images/logo-spitball.svg';
import notification from "./images/notification-icon.svg";

export default {
    components: {
        'search-type': searchTypes,
        askHeader,
        bookHeader,
        noteHeader,
        flashcardHeader,
        jobHeader,
        foodHeader,
        tutorHeader,
        logo,
        "notification-icon" : notification,
        "search-icon": searchIcon,
        "bookDetailsHeader":bookHeader
    },
    data() {
        return {
            placeholders: placeholders,
            showOption: false,
            names: names,
            qFilter: this.userText,
            isOptions: false,
            snackbar:true
        }
    },

    props: {
        openOptions: { type: Function }, userText: { type: String }, name: {type:String}
    },

    watch: {
        '$route.query': '$_routeChange'
    },

    methods: {
        ...mapActions(['updateSearchText']),
        $_routeChange(val) {
            this.qFilter = this.userText;
        },
        changeType: function (val) {
            this.qFilter = "";
            this.$refs.search.focus();
        },
        submit: function () {
            this.isOptions = false;
            this.updateSearchText(this.qFilter).then((response) => {
                this.$router.push({ path:response, query: { q: this.qFilter } });
            });
            this.$emit('update:overlay', false);
        },
        focus: function () {
            this.isOptions = true;
            this.$emit('update:overlay', true);
        }
    }
}
