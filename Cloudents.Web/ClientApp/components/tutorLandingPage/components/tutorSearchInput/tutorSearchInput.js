import tempSuggestData from "../../suggestData"

import debounce from "lodash/debounce";
import { mapGetters } from 'vuex';
import { LanguageService } from "../../../../services/language/languageService";

export default {
    name: "tutor-search-input",
    props: {
        placeholder:{
            type:String,
            required:false,
            default: "some placeholder"
        }
    },
    data: () => ({
        autoSuggestList: [],
        isFirst: true,
        showSuggestions: false,
        originalMsg: '',
        isRtl: global.isRtl,
        msg:"",
        suggest: tempSuggestData
    }),
    computed: {
        ...mapGetters(['allHistorySet', 'getCurrentVertical', 'getVerticalHistory']),
        isSearchActive() {
            if(this.$vuetify.breakpoint.xsOnly) {
                return !!this.$route.query && !!this.$route.query.term;
            } else {
                return false;
            }

        },
        suggestList() {
            //TODO GET THE SUGGEST LIST FROM THE SERVER
            
            return [];
        }
    },
    watch: {
        userText(val) {
            this.msg = val;
            this.isFirst = true;
            this.showSuggestions = true;
        },
        '$route'() {
            this.$forceUpdate();
        }
    },
    methods: {
        goBackFromSearch() {
            this.msg = "";
            this.search();
        },
        outsideClick() {
            console.log('clicked outside');
            this.closeSuggestions();
        },
        search() {
            this.closeSuggestions();
            // to remove keyboard on mobile
            this.$nextTick(() => {
                this.$el.querySelector('input').blur();
            });
        },
        openSuggestions() {            
            this.showSuggestions = true;
        },
        changeMsg: debounce(function (val) {
            if(!!val && val.length > 0) {
                this.openSuggestions();
            } else {
                this.closeSuggestions();
            }

        }, 250),
        closeSuggestions() {
            //this.$el.querySelector('.search-b input').blur();
            //this.msg = this.originalMsg;
            if(this.showSuggestions) {
                this.showSuggestions = false;
                //this.$el.querySelector('.search-menu').scrollTop = 0;
            }
        }
    },
    created() {
    }

};