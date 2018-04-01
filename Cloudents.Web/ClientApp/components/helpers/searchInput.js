import debounce from "lodash/debounce"

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
    data: () => ({autoSuggestList: [], isFirst: true, showSuggestions: false, focusedIndex: -1, originalMsg: ''}),
    computed: {
        ...mapGetters({'globalTerm': 'currentText'}),
        ...mapGetters(['allHistorySet', 'getCurrentVertical', 'getVerticalHistory']),
        suggestList() {
            let currentHistory = this.getCurrentVertical;
            let buildInSuggestList = currentHistory ? consts.buildInSuggest[currentHistory] : consts.buildInSuggest.home;
            let historyList = [...(this.submitRoute && currentHistory ? this.$store.getters.getVerticalHistory(currentHistory) : this.allHistorySet)];
            let historySuggestSet = [...new Set([...historyList, ...buildInSuggestList])];
            let set = currentHistory ? historySuggestSet : [...new Set([...this.autoSuggestList, ...historySuggestSet])];
            let autoListMap = this.autoSuggestList ? this.autoSuggestList.map((i) => ({
                text: i,
                type: consts.SUGGEST_TYPE.autoComplete
            })) : [];
            let mapDataSet = set.slice(0, this.maxResults).map(i => ({
                text: i, type: (this.autoSuggestList.includes(i) ? consts.SUGGEST_TYPE.autoComplete :
                    historyList.includes(i) ? consts.SUGGEST_TYPE.history :
                        consts.SUGGEST_TYPE.buildIn)
            }));
            return currentHistory ? [...autoListMap, ...mapDataSet] : mapDataSet;
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
            this.isFirst = true;
        },
        msg: debounce(function (val) {
            if(this.focusedIndex >= 0 && this.msg !== this.suggestList[this.focusedIndex].text){
                this.focusedIndex = -1;
            }
            if (this.focusedIndex < 0) {
                this.$emit('input', val);
                if (val && !this.isFirst) {
                    this.getAutocmplete(val).then(({data}) => {
                        this.autoSuggestList = data ? data.result : []
                    })
                } else {
                    this.autoSuggestList = [];
                }
                this.isFirst = false;
            }
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
            if (this.focusedIndex >= 0) {
                this.msg = this.suggestList[this.focusedIndex].text;
            }
            if (this.submitRoute) {
                this.$router.push({path: this.submitRoute, query: {q: this.msg}});
            }
            else if (this.msg) {
                this.$router.push({name: "result", query: {q: this.msg}});
            }
            this.closeSuggestions();
            // to remove keyboard on mobile
            this.$nextTick(() => {
                this.$el.querySelector('input').blur();
            });
        },
        openSuggestions() {
            this.showSuggestions = true;
            if (this.$root.$el.querySelector('.box-search')) { // Limit height Only in home page
                var rect = this.$root.$el.querySelector('.box-search').getBoundingClientRect();
                this.$el.querySelector('.search-menu').style.maxHeight = (window.innerHeight - rect.top - rect.height - 4) + "px";
            }
        },
        closeSuggestions() {
            this.$el.querySelector('.search-b input').blur();
            this.focusedIndex = -1;
            this.msg = this.originalMsg;
            if (this.showSuggestions) {
                this.showSuggestions = false;
                this.$el.querySelector('.search-menu').scrollTop = 0;
            }
        },
        onScroll(e) {
            if (this.hideOnScroll && this.showSuggestions) {
                let rect = this.$root.$el.querySelector('.search-menu').getBoundingClientRect();
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
        },
        arrowNavigation(direction) {
            // When to save user's typed text
            if (this.focusedIndex === -1) {
                this.originalMsg = this.msg;
            }

            // Handling arrows:
            if (this.focusedIndex < 0 && direction < 0) {
                this.focusedIndex = this.suggestList.length - 1;
            }
            else {
                this.focusedIndex = this.focusedIndex + direction;
            }

            // Out of bounds - set index to be -1:
            if (this.focusedIndex === this.suggestList.length || this.focusedIndex < 0) {
                this.focusedIndex = -1;
                this.msg = this.originalMsg;
            }

            // In bounds - set input to be the highlighted suggestion
            else {
                this.msg = this.suggestList[this.focusedIndex].text;
            }

        }
    },
    created() {
        if (!this.isHome) {
            this.msg = this.userText ? this.userText : this.globalTerm ? this.globalTerm : "";
        }
    }
}