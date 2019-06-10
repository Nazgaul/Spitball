

import debounce from "lodash/debounce";
import { LanguageService } from "../../../../services/language/languageService";
import universityService from "../../../../services/universityService";
import analyticsService from '../../../../services/analytics.service';

export default {
    name: "tutor-search-input",
    props: {
        placeholder:{
            type:String,
            required:false,
            default: LanguageService.getValueByKey('tutorListLanding_search_placeholder'),
        }
    },
    data: () => ({
        autoSuggestList: [],
        isFirst: true,
        showSuggestions: false,
        originalMsg: '',
        isRtl: global.isRtl,
        msg:"",
        suggests: [],
        term: '',
    }),
    computed: {
    },
    watch: {
        '$route'() {
            this.$forceUpdate();
        }
    },
    methods: {
        outsideClick() {
            console.log('clicked outside');
            this.closeSuggestions();
        },
        search(text) {
            if(!!text){
                this.msg = text;
            }
            if(!!this.msg){
                analyticsService.sb_unitedEvent("Tutor_Engagement", "Search", this.msg);
            }
            let query = {term: this.msg}
            this.$router.push({path: 'tutor-list', query})
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
                this.getSuggestionList(val);
            } else {
                this.closeSuggestions();
            }

        }, 250),
        closeSuggestions() {
            if(this.showSuggestions) {
                this.showSuggestions = false;
            }
        },
        getSuggestionList(term){
            universityService.getCourse({term, page:0}).then(data=>{
                this.suggests = data;
            }).finally(()=>{
                this.openSuggestions();
            })
            
        },
        selectors(item){
            this.search(item.text);
        }
    },
    created() {
        this.msg = !!this.$route.query && !!this.$route.query.term ? this.$route.query.term : '';
    }

};