import micIcon from "./svg/mic.svg";
import searchIcon from "./svg/search-icon.svg";
import classMaterialIcon from "./svg/class-material-icon-purple.svg";
import tutorIcon from "./svg/tutor-icon-green.svg";
import jobIcon from "./svg/jobs-icon-orange.svg";
import textBookIcon from "./svg/textbooks-icon-purple.svg";

import facebookIcon from "./svg/facebook-icon.svg";
import twitterIcon from "./svg/twitter-icon.svg";
import googleIcon from "./svg/google-icon.svg";
import youtubeIcon from "./svg/youtube-icon.svg";
import instegramIcon from "./svg/instagram-icon.svg";
import menuIcon from "./svg/menu-icon.svg";
import { verticalsPlaceholders as askPlaceholder } from "./../data";

let homeSuggest = [
    'Flashcards for financial accounting',
    'Class notes for my Calculus class',
    'When did World War 2 end?',
    'Difference between meiosis and mitosis',
    'Tutor for Linear Algebra',
    'Job in marketing in NYC',
    'The textbook - Accounting: Tools for Decision Making',
    'Where can I get a burger near campus?'
];
export default {
    components: {
        "mic-icon": micIcon, "search-icon": searchIcon,
        "class-material-icon": classMaterialIcon,
        "tutor-icon": tutorIcon,
        "job-icon": jobIcon,
        "text-book-icon": textBookIcon,
        "facebook-icon": facebookIcon,
        "twitter-icon": twitterIcon,
        "google-icon": googleIcon,
        "youtube-icon": youtubeIcon,
        "instegram-icon": instegramIcon,menuIcon
    },
    data() {
        return {
            placeholder: askPlaceholder.ask,
            items: homeSuggest,
            msg: '',
            drawer: null,
            links: [
                {
                    name: "Spitball Guide",
                    link: "#"
                },
                {
                    name: "Key Features",
                    link: "#"
                },
                {
                    name: "Shared Documents",
                    link: "#"
                },
                {
                    name: "Mobile App",
                    link: "#"
                }
            ]
        };
    },
    methods: {
        $_voiceDetection() {
            var recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition || window.mozSpeechRecognition || window.msSpeechRecognition)();
            recognition.lang = 'en-US';
            recognition.interimResults = false;
            recognition.maxAlternatives = 5;
            recognition.start();
            var _self = this;

            recognition.onresult = function (event) {
                _self.msg = event.results[0][0].transcript;
            };
        },
        search(){
            this.$store.dispatch('updateSearchText', this.msg ).then((name) => {
                this.$router.push({ path:'/'+name, query: {q:this.msg} });
            });;
        },
        selectos(item) {
            this.msg = item;
            this.search();
        }
    },

    props: {
         metaText: { type: String }
    },

    computed: {
        voiceEnable() { return window.chrome}
    }
};
