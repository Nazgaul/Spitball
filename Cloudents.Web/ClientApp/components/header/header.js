import {  names } from "../data"
import logo from "../../../wwwroot/Images/logo-spitball.svg";
import { mapActions, mapGetters } from 'vuex';
import { settingMenu } from '../settings/consts';
//import 'vue-awesome/icons/search';
//import VIcon from 'vue-awesome/components/Icon.vue'

export default {
    components: {
        logo
    },
    data() {
        return {
            settingMenu,
           // placeholders: placeholders,
            names: names,
            currentName:"",
            qFilter: this.$route.query.q,
            snackbar:true
        };
    },

    computed: {
        ...mapGetters(['luisTerm','getUniversityName']),
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
        },
        isMobileSize: function () {
            return this.$vuetify.breakpoint.xsOnly;
        },
        voiceAppend(){
           return ("webkitSpeechRecognition" in window)?'sbf-mic':'';
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
                this.$router.push({ path: result, query: { q: this.qFilter } });
            });
            this.$emit("update:overlay", false);
        },
        menuToggle: function() {
            this.$emit("input",!this.value);
        },
        mic: function () {
            //TODO: YIFAT need to add mic handle here.
            console.log("hi")
        }
    }
}
