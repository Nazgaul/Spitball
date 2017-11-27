import searchIcon from "./Images/search-icon.svg";
import { mapActions} from "vuex"
import {  names } from "../data"
import logo from "../../../wwwroot/Images/logo-spitball.svg";
import settingsIcon from "./Images/settings-icon.svg";

export default {
    components: {
        //'search-type': searchTypes,
        logo,
        settingsIcon,
        "search-icon": searchIcon
    },
    data() {
        return {
           // placeholders: placeholders,
            names: names,
            currentName:"",
            qFilter: this.$route.query.q,
            snackbar:true
        };
    },

    computed: {
        name: function () {
            let currentPage = this.$route.meta.pageName ? this.$route.meta.pageName : this.$route.path.split("/")[1];
            if (this.currentName !== currentPage) {
                this.currentName = currentPage;
                    if (this.$route.query.q) {
                        this.qFilter = this.$route.query.q;
                        this.$emit("update:userText", this.qFilter);
                    }
                }
                return this.currentName;
        }
    },

    props:{value:{type:Boolean}},
    methods: {
        ...mapActions(["updateSearchText"]),
        submit: function () {
            this.updateSearchText(this.qFilter).then((response) => {
                this.$router.push({ path: response, query: { q: this.qFilter } });
                this.$emit("update:userText", this.qFilter);
            });
            this.$emit("update:overlay", false);
        },
        menuToggle: function() {
            this.$emit("input",!this.value);
        }
    }
}
