import debounce from "lodash/debounce";
import historyIcon from "./svg/history-icon.svg";
import {mapGetters, mapActions} from 'vuex'
import * as consts from './consts';
import { constants } from "../../utilities/constants";

export default {
    name: "search-input",
    components: {historyIcon},
    props: {
        hideOnScroll: {type: Boolean, default: false},
        placeholder: {type: String},
        userText: {String},
        submitRoute: {String,default:'/ask'},
        suggestionVertical: {String}
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
            let autoListMap = this.autoSuggestList ? this.autoSuggestList.map((i) => ({
                text: i,
                type: consts.SUGGEST_TYPE.autoComplete
            })) : [];
            let mapDataSet = historySuggestSet.slice(0, this.maxResults).map(i => ({
                text: i, type: (historyList.includes(i) ? consts.SUGGEST_TYPE.history :
                        consts.SUGGEST_TYPE.buildIn)
            }));
            return [...autoListMap, ...mapDataSet];
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
            if (this.focusedIndex >= 0 && this.msg !== this.suggestList[this.focusedIndex].text) {
                this.focusedIndex = -1;
            }
            if (this.focusedIndex < 0) {
                this.originalMsg = this.msg;
                this.$emit('input', val);
                if (val && !this.isFirst) {
                    this.getAutocmplete({term: val, vertical:this.suggestionVertical ? this.suggestionVertical : this.getCurrentVertical}).then(({data}) => {
                        this.autoSuggestList = data
                    })
                } else {
                    this.autoSuggestList = [];
                }
                this.isFirst = false;
            }
        }, 250),
        focusedIndex(val) {
            if (val < 0) {
                this.msg = this.originalMsg;
            }
            else {
                this.msg = this.suggestList[this.focusedIndex].text;
            }
        }
    },
    methods: {
        ...mapActions(['getAutocmplete']),
        selectos({item, index}) {
            this.msg = item.text;
            this.$ga.event('Search_suggestions', `Suggest_${this.getCurrentVertical ? this.getCurrentVertical.toUpperCase() : 'HOME'}_${item.type}`, `#${index + 1}_${item}`);
            this.search();
            this.closeSuggestions();
        },
        search() {
            if (!constants.regExXSSCheck.test(this.msg)){
                this.$router.push({path: this.submitRoute, query: {q: this.msg}});
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
        highlightSearch: function (item) {
            if (!item.type === consts.SUGGEST_TYPE.autoComplete || !this.msg) {
                if (!constants.regExXSSCheck.test(item.text)){
                    return item.text
                }else{
                    return "";
                }
            }
            else {
                let term = this.msg.toLowerCase();
                let itemLower = item.text.toLowerCase();
                let matchStartIndex = itemLower.indexOf(term);
                if (matchStartIndex < 0) {
                    return item.text;
                }
                let matchEndIndex = matchStartIndex + term.length;
                return item.text.slice(0, matchStartIndex)
                    + '<span class=\'highlight\'>' + item.text.slice(matchStartIndex, matchEndIndex) + '</span>'
                    + item.text.slice(matchEndIndex, item.text.length);
            }
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
            }

        }
    },
    created() {
        if (!this.isHome) {
            this.msg = this.userText ? this.userText : this.globalTerm ? this.globalTerm : "";
        }
    }
}