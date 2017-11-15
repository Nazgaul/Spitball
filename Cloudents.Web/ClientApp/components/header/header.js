import askHeader from '../navbar/images/ask.svg'
import flashcardHeader from '../navbar/images/flashcard.svg'
import jobHeader from '../navbar/images/job.svg'
import bookHeader from '../navbar/images/book.svg'
import noteHeader from '../navbar/images/document.svg'
import tutorHeader from '../navbar/images/tutor.svg'
import courseHeader from '../navbar/images/courses.svg'
import foodHeader from '../navbar/images/food.svg'
// import settingHeader from '../navbar/images/'
import searchTypes from './../helpers/radioList.vue'
import searchIcon from './Images/search-icon.svg';
import hamburger from './Images/hamburger.svg';
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
        courseHeader,
        jobHeader,
        foodHeader,
        tutorHeader,
        logo,
        "notification-icon" : notification,
        "search-icon": searchIcon,
        "bookDetailsHeader": bookHeader,
        hamburger
    },
    data() {
        return {
            placeholders: placeholders,
            showOption: false,
            names: names,
            currentName:'',
            qFilter: this.$route.query.q,
            isOptions: false,
            snackbar:true
        };
    },

    computed: {
        name: function () {
            let currentPage = this.$route.meta.pageName ? this.$route.meta.pageName : this.$route.path.split('/')[1];
            if (this.currentName !== currentPage) {
                this.currentName = currentPage;
                    if (this.$route.query.q) {
                        this.qFilter = this.$route.query.q;
                        this.$emit('update:userText', this.qFilter);
                    }
                }
                return this.currentName;
        }
    },

    methods: {
        ...mapActions(['updateSearchText']),
        submit: function () {
            this.updateSearchText(this.qFilter).then((response) => {
                this.$router.push({ path: response, query: { q: this.qFilter } });
                this.$emit('update:userText', this.qFilter);
            });
            this.$emit('update:overlay', false);
        },
        menuToggle: function() {
            console.log("here")

        }
    }
}
