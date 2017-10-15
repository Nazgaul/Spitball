import verticalCollection1 from './../general/vertical-collection.vue';
import search from "./../../api/ai";

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


export default {
    components: {
        'vertical-collection': verticalCollection1,
        "mic-icon": micIcon, "search-icon": searchIcon,
        "class-material-icon": classMaterialIcon,
        "tutor-icon": tutorIcon,
        "job-icon": jobIcon,
        "text-book-icon": textBookIcon,
        "facebook-icon": facebookIcon,
        "twitter-icon": twitterIcon,
        "google-icon": googleIcon,
        "youtube-icon": youtubeIcon,
        "instegram-icon": instegramIcon
    },
    data() {
        var prefix;
        return {
            msg: this.$route.meta.userText,
            placeholder: "",
            searchType:"",
            drawer: null,
            actions: [
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
            ],
            changeSection: (vertical) => {
                this.placeholder = vertical.placeholder;
                prefix = vertical.prefix;
                this.searchType = vertical.name;
            },
            search: () => {
                this.$route.meta.searchType = this.searchType;
                this.$store.dispatch('updateSearchText', { prefix: prefix, str: this.msg });
            }
        };
    }
};
