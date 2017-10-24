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
import { homeSuggest, verticalsPlaceholders as askPlaceholder } from "./../data";


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
            showOptions: false,
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
        search(){
            this.$store.dispatch('updateSearchText', this.msg ).then((name) => {
                this.$router.push({ path:'/'+name, query: {q:this.msg} });
            });;
        },
        $_focus() {
            console.log('focus')
            this.showOptions = true;
        },
        selectos(item) {
            console.log("selected " + item)
            this.msg = item;
            this.search();
        }
    },

    props: {
         metaText: { type: String }
    }
};
