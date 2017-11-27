import searchIcon from "./Images/search-icon.svg";
import { mapActions} from "vuex"
import {  names } from "../data"
import logo from "../../../wwwroot/Images/logo-spitball.svg";
import settingsIcon from "./Images/settings-icon.svg";
import { mapActions,mapGetters} from 'vuex'

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
        ...mapGetters(['luisTerm']),
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
    watch:{
      '$route':function(val){
          this.qFilter=val.query.q;
      }
    },
    props:{value:{type:Boolean}},
    methods: {
        ...mapActions(["updateSearchText"]),
        submit: function () {
            this.updateSearchText(this.qFilter).then((response) => {
                let result=response.result;
                this.$route.meta[result.includes('food')?'foodTerm':result.includes('job')?'jobTerm':'term']={
                    term: this.qFilter,
                    luisTerm: response.term
                };
                this.$router.push({ path: response.result, query: { q: this.qFilter } });
            });
            this.$emit("update:overlay", false);
        },
        menuToggle: function() {
            this.$emit("input",!this.value);
        }
    }
}
