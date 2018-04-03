import debounce from "lodash/debounce"

    ;
import historyIcon from "./svg/history-icon.svg";
import {micMixin} from './mic';
import {mapGetters, mapActions} from 'vuex'
import * as consts from './consts';
import {searchObjects} from "../settings/consts";

export default {
    name: "search-input",
    mixins: [micMixin],
    components: {historyIcon},
    props: {
        hideOnScroll: {type: Boolean, default: false},
        placeholder: {type: String},
        userText: {String},
        submitRoute: {String},
        searchType: {String, default: 'term'},
        disabled: {type: Boolean},
        searchOnSelection: {type: Boolean, default: true},
        searchVertical: {type: String, default: ""}
    },
    data: () => ({
        autoSuggestList: [],
        uniAutocompleteList: [],
        uniSuggestList: [],
        uniList: [],
        isFirst: true,
        showSuggestions: false
    }),
    computed: {
        ...mapGetters({'globalTerm': 'currentText'}),
        ...mapGetters(['allHistorySet', 'getCurrentVertical', 'getVerticalHistory', 'getUniversityName']),
        suggestList() {
            if (this.searchType && this.searchType === 'uni') {
                this.uniList = [...new Set([...this.uniAutocompleteList, ...this.uniSuggestList])]
                return this.uniList.slice(0, this.maxResults).map(i => ({
                    text: i.name, image: i.image, type: consts.SUGGEST_TYPE.autoComplete
                }));
            }
            else {//term
                let currentHistory = this.getCurrentVertical ? this.getCurrentVertical : this.searchVertical;
                let buildInSuggestList = currentHistory ? consts.buildInSuggest[currentHistory] : consts.buildInSuggest.home;
                let historyList = [...(this.submitRoute && currentHistory ? this.$store.getters.getVerticalHistory(currentHistory) : this.allHistorySet)];
                let set = [...new Set([...this.autoSuggestList, ...historyList, ...buildInSuggestList])];
                return set.slice(0, this.maxResults).map(i => ({
                    text: i, type: (this.autoSuggestList.includes(i) ? consts.SUGGEST_TYPE.autoComplete :
                        historyList.includes(i) ? consts.SUGGEST_TYPE.history :
                            consts.SUGGEST_TYPE.buildIn)
                }));
            }
        },

        isHome() {
            return this.$route.name === 'home'
        },
        maxResults() {
            return this.isHome ? consts.HOME_MAX_SUGGEST_NUM : consts.VERTICAL_MAX_SUGGEST_NUM
        }
    },
    watch: {
        userText(val) {
            // debugger;
            // if(this.searchType !=='uni' || (this.searchType ==='uni' && this.suggestList.filter(suggestion => (suggestion.text === this.msg)).length)) {
            this.msg = val;
            // }
            this.isFirst = true;
        },
        msg: debounce(function (val) {
                this.$emit('input', val);
                if (this.searchType && this.searchType === 'uni') {
                    this.$store.dispatch("getUniversities", {term: val}).then(({data}) => {
                        this.uniAutocompleteList = val ? data : [];
                    });
                }
                else {
                    if (this.msg && !this.isFirst) {
                        this.getAutocmplete({term: val, vertical:this.searchVertical ? this.searchVertical : this.getCurrentVertical}).then(({data}) => {
                            this.autoSuggestList = val ? data : [];
                        });

                    }
                    else {
                        this.autoSuggestList = [];
                    }
                    this.isFirst = false;
                }
            }
            ,
            250
        )
    },
    methods: {
        ...mapActions(['getAutocmplete', 'updateUniversity']),
        selectos({item, index}) {
            if (this.searchType && this.searchType === 'uni') {
                this.updateUniversity(this.uniList[index]);
            }
            else {
                this.$ga.event('Search', `Suggest_${this.getCurrentVertical ? this.getCurrentVertical.toUpperCase() : 'HOME'}_${item.type}`, `#${index + 1}_${item}`);
            }
            this.msg = item.text;
            this.closeSuggestions();
            if (this.searchOnSelection) {
                this.search();
            }
        }
        ,
        search() {
            if (!this.msg){
                return;
            }
            if (this.submitRoute) {
                if (this.searchType !== 'uni') {
                        this.$router.push({path: this.submitRoute, query: {q: this.msg}});
                }
                else {
                    let matchingSuggestions = this.suggestList.filter(suggestion => (suggestion.text === this.msg))
                    if (matchingSuggestions.length) {
                        this.updateUniversity(this.uniList[this.suggestList.indexOf(matchingSuggestions[0])]);
                        this.$router.push({path: this.submitRoute, query: {q: ''}});
                    }
                }
            }
            else {
                this.$router.push({name: "result", query: {q: this.msg}});
            }
            this.closeSuggestions();
            // to remove keyboard on mobile
            this.$nextTick(() => {
                this.$el.querySelector('input').blur();
            });
        }
        ,
        openSuggestions() {
            this.showSuggestions = true;
            if (this.$root.$el.querySelector('.box-search')) { // Limit height Only in home page
                var rect = this.$root.$el.querySelector('.box-search').getBoundingClientRect();
                this.$el.querySelector('.search-menu').style.maxHeight = (window.innerHeight - rect.top - rect.height - 4) + "px";
            }
            this.$emit('openedSuggestions', true);
        }
        ,
        closeSuggestions() {
            this.$el.querySelector('.search-b input').blur();
            if (this.showSuggestions) {
                this.showSuggestions = false;
                this.$el.querySelector('.search-menu')
                if (this.$el.querySelector('.search-menu').length) {
                    this.$el.querySelector('.search-menu').scrollTop = 0;
                }
            }
            this.$emit('openedSuggestions', false);
        }
        ,
        onScroll(e) {
            if (this.hideOnScroll && this.showSuggestions) {
                var rect = this.$root.$el.querySelector('.search-menu').getBoundingClientRect();
                if (rect.top < -rect.height) {
                    this.closeSuggestions();
                }
            }
        }
        ,
        //callback for mobile submit mic
        submitMic() {
            this.search();
        }
        ,
        highlightSearch: function (item) {
            let term = this.msg;
            let regex = /(<([^>]+)>)/ig;
            return item.type === consts.SUGGEST_TYPE.autoComplete ? item.text.replace(term, '<span class=\'highlight\'>' + term + '</span>') : item.text.replace(regex, "");
        }
    }
    ,
    created() {
        if (!this.isHome && !(this.searchType && this.searchType === 'uni')) {
            this.msg = this.userText ? this.userText : this.globalTerm ? this.globalTerm : "";
        }
        if (this.searchType && this.searchType === 'uni') {
            let uniName = this.$store.getters.getUniversityName;
            if (uniName) {
                this.msg = uniName;
            } else {
                this.$store.dispatch("getUniversities", {term: ''}).then(({data}) => {
                    this.uniSuggestList = data;
                });
            }
        }
    }
}