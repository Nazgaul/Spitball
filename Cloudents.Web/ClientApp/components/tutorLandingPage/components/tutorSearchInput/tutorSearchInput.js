

import debounce from "lodash/debounce";
import { LanguageService } from "../../../../services/language/languageService";
import courseService from "../../../../services/courseService";
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
        showSuggestions: false,
        originalMsg: '',
        msg:"",
        suggests: [],
        term: '',
        focusedIndex: -1,
    }),
    computed: {
    },
    watch: {
        '$route'() {
            this.$forceUpdate();
        }
    },
    methods: {
        search(text) {
            if(!!text){
                this.msg = text;
            }else if(!this.msg) {
                this.msg = '';
            }
            if(!!this.msg){
                analyticsService.sb_unitedEvent("Tutor_Engagement", "Search", this.msg);
            }

            //this.$router.push({ path: `/tutor-list/${encodeURIComponent(this.msg)}` }).catch(() => {});
            this.$router.push({ name: 'tutorLandingPage', params: {course: this.msg} }).catch(() => {});
            
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
                this.focusedIndex = -1;
                this.originalMsg = "";
                this.showSuggestions = false;
            }
        },
        getSuggestionList(term){
            courseService.getCourse({term, page:0}).then(data=>{
                this.suggests = data;
            }).finally(()=>{
                this.openSuggestions();
            });

        },
        selectors(item){
            this.search(item.text);
        },
        focusOnSelectedElm(searchMenu, direction){
            const ELM_HEIGHT_WITH_MARGIN = 51;
            const ELM_HEIGHT_WITHOUT_MARGIN = 36;
            let containerHeight = searchMenu.parentElement.offsetHeight;
            let currentHighlightedElm = searchMenu.offsetTop + ELM_HEIGHT_WITH_MARGIN;
            if(direction === 1) {
                if((currentHighlightedElm + ELM_HEIGHT_WITHOUT_MARGIN) > containerHeight){
                    let stepsToScroll = Math.floor(currentHighlightedElm / ELM_HEIGHT_WITH_MARGIN);
                    let newScrollVal = stepsToScroll * ELM_HEIGHT_WITH_MARGIN;
                    searchMenu.parentElement.scrollTop = newScrollVal;
                } else {
                    searchMenu.parentElement.scrollTop = 0;
                }
            } else {
                if(searchMenu.parentElement.scrollTop > 0 && Math.floor(searchMenu.parentElement.scrollTop/ELM_HEIGHT_WITH_MARGIN) > 1) {
                    currentHighlightedElm = currentHighlightedElm - ELM_HEIGHT_WITH_MARGIN;
                    let stepsToScroll = Math.floor(currentHighlightedElm / ELM_HEIGHT_WITH_MARGIN);
                    let newScrollVal = stepsToScroll * ELM_HEIGHT_WITH_MARGIN;
                    searchMenu.parentElement.scrollTop = newScrollVal - ELM_HEIGHT_WITH_MARGIN;
                } else if((currentHighlightedElm - ELM_HEIGHT_WITHOUT_MARGIN) < containerHeight) {
                    searchMenu.parentElement.scrollTop = 0;
                } else {
                    searchMenu.parentElement.scrollTop = currentHighlightedElm * ELM_HEIGHT_WITH_MARGIN;
                }
            }
        },
        arrowNavigation(direction) {
            // When to save user's typed text

            let searchMenu = document.querySelector('.list__tile--highlighted');

            // saving user input text
            if (this.focusedIndex === -1) {
                this.originalMsg = this.msg;
            }

            // Handling arrows:

            if(direction > 0) {
                if(this.suggests.length -1 === this.focusedIndex) {
                    return;
                }
                this.focusedIndex = this.focusedIndex + direction;
            }  else { 
                if(this.focusedIndex < 0) {
                    return;
                } 
                this.focusedIndex = this.focusedIndex + direction;
            }
        
            if(this.focusedIndex > -1 && this.focusedIndex < this.suggests.length) {
                this.msg = this.suggests[this.focusedIndex].text;
            }

            if(searchMenu){
                this.focusOnSelectedElm(searchMenu, direction);
            } else {
                return;
            }
            
            // Out of bounds - set index to be -1:
            if (this.focusedIndex === this.suggests.length || this.focusedIndex < 0) {
                this.msg = this.originalMsg;
            }
        }
    },
    created() {
        this.msg = !!this.$route.params && !!this.$route.params.course ? this.$route.params.course : '';
    }

};