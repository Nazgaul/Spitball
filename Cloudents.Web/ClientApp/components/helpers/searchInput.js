﻿import debounce from "lodash/debounce"

;
import historyIcon from "./svg/history-icon.svg";
import {micMixin} from './mic';
import {mapGetters, mapActions} from 'vuex'
import * as consts from './consts';

export default {
    name: "search-input",
    mixins: [micMixin],
    components: {historyIcon},
    props: {
        hideOnScroll: {type: Boolean, default: false},
        placeholder: {type: String},
        userText: {String},
        submitRoute: {String}
    },
    data:()=>({autoSuggestList:[],isFirst:true, showSuggestions: false}),
    computed: {
        ...mapGetters({'globalTerm': 'currentText'}),
        ...mapGetters(['allHistorySet', 'getCurrentVertical', 'getVerticalHistory']),
        suggestList() {
            let currentHistory = this.getCurrentVertical;
            let buildInSuggestList = currentHistory ? consts.buildInSuggest[currentHistory] : consts.buildInSuggest.home;
            let historyList = [...(this.submitRoute && currentHistory ? this.$store.getters.getVerticalHistory(currentHistory) : this.allHistorySet)];
            let set = [...new Set([...this.autoSuggestList, ...historyList, ...buildInSuggestList])];
            return set.slice(0, this.maxResults).map(i => ({
                text: i, type: (this.autoSuggestList.includes(i) ? consts.SUGGEST_TYPE.autoComplete :
                    historyList.includes(i) ? consts.SUGGEST_TYPE.history :
                        consts.SUGGEST_TYPE.buildIn)
            }));
        },

        isHome() {
            return this.$route.name === 'home'
        },
        maxResults() {
            return this.isHome ? consts.HOME_MAX_SUGGEST_NUM : consts.VERTICAL_MAX_SUGGEST_NUM
        },
    },
    watch: {
        userText(val) {
            this.msg = val;
            this.isFirst=true;
        },
        msg:debounce(function (val) {
            this.$emit('input',val);
            if(this.msg&&!this.isFirst){
                this.getAutocmplete(val).then(({data})=>{this.autoSuggestList=val?data:[]})}else{this.autoSuggestList=[];}
            this.isFirst=false;
        }, 250)
    },
    methods: {
        ...mapActions(['getAutocmplete']),
        selectos({item, index}) {
            this.msg = item.text;
            this.$ga.event('Search', `Suggest_${this.getCurrentVertical ? this.getCurrentVertical.toUpperCase() : 'HOME'}_${item.type}`, `#${index + 1}_${item}`);
            this.search();
            this.closeSuggestions();
        },
        search() {
            if (this.submitRoute) {
                this.$router.push({path: this.submitRoute, query: {q: this.msg}});
            }
            else if (this.msg) {
                this.$router.push({name: "result", query: {q: this.msg}});
            }
            // to remove keyboard on mobile
            this.$nextTick(() => {
                this.$el.querySelector('input').blur();
            });
        },
        openSuggestions() {
            this.showSuggestions = true;
            if(this.$root.$el.querySelector('.box-search')) { // Limit height Only in home page
                var rect = this.$root.$el.querySelector('.box-search').getBoundingClientRect();
                this.$el.querySelector('.search-menu').style.maxHeight = (window.innerHeight - rect.top - rect.height - 4) + "px";
            }
        },
        closeSuggestions() {
            this.$el.querySelector('.search-b input').blur();
            if (this.showSuggestions) {
                this.showSuggestions = false;
                this.$el.querySelector('.search-menu').scrollTop = 0;
            }
        },
        onScroll(e) {
            if (this.hideOnScroll && this.showSuggestions) {
                var rect = this.$root.$el.querySelector('.search-menu').getBoundingClientRect();
                if (rect.top < -rect.height) {
                    this.closeSuggestions();
                }
            }
        },
        //callback for mobile submit mic
        submitMic() {
            this.search();
        },
        highlightSearch: function (item) {
            let term = this.msg;
            let regex = /(<([^>]+)>)/ig;
            let aa = item.type === consts.SUGGEST_TYPE.autoComplete ? item.text.replace(term, '<span class=\'highlight\'>' + term + '</span>') : item.text.replace(regex, "");
            return aa;
        }
    },
    created() {
        if (!this.isHome) {
            this.msg = this.userText ? this.userText : this.globalTerm;
        }
    }
}