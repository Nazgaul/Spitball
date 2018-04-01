import debounce from "lodash/debounce";
import {mapGetters, mapActions} from 'vuex'

export default {
    name: "uni-search-input",
    props: {
        placeholder: {type: String},
    },
    data: () => ({
        msg: "",
        uniSuggestList: [],
        showSuggestions: false,
        focusedIndex: -1,
    }),
    computed: {
        ...mapGetters(['getUniversityName']),
        suggestList() {
            return this.uniSuggestList.slice(0, 3);
        },
    },
    watch: {
        msg: debounce(function (val) {
            debugger;
            this.$emit('input', val);
            this.$store.dispatch("getUniversities", {term: val}).then(({data}) => {
                let {universities} = data;
                this.uniSuggestList = val ? universities : [];
            });
        }, 250)
    },
    methods: {
        ...mapActions(['getAutocmplete', 'updateUniversity']),
        selectos({item, index}) {
            this.updateUniversity(this.uniSuggestList[index]);
            this.closeSuggestions();
            this.$router.push({path: '/note', query: {q: ''}});
            // to remove keyboard on mobile
            this.$nextTick(() => {
                this.$el.querySelector('input').blur();
            });
        },
        search() {
            debugger;
            if (!this.msg) {
                return;
            }
            var index = this.uniSuggestList.findIndex(uni => uni.name == this.msg);
            if(index >= 0) {
                this.selectos({index: index});
            }
        },
        openSuggestions() {
            this.showSuggestions = true;

        },
        closeSuggestions() {
            this.$el.querySelector('.search-b input').blur();
            this.focusedIndex = -1;
            if (this.showSuggestions) {
                this.showSuggestions = false;
                this.$el.querySelector('.search-menu').scrollTop = 0;
            }
        },
        highlightSearch: function (item) {
            let term = this.msg;
            let uniLower = item.name.toLowerCase();
            let matchStartIndex = uniLower.indexOf(term);
            if (!matchStartIndex < 0) {
                return item.name;
            }
            let matchEndIndex = matchStartIndex + term.length;
            return item.name.slice(0, matchStartIndex)
                + '<span class=\'highlight\'>' + item.name.slice(matchStartIndex, matchEndIndex) + '</span>'
                + item.name.slice(matchEndIndex, item.name.length);

        },
        arrowNavigation(direction) {
            if (direction > 0 && this.focusedIndex === this.uniSuggestList.length - 1 || direction < 0 && this.focusedIndex === 0) {
                return;
            }
            this.focusedIndex = this.focusedIndex + direction
        }
    },
    created() {
        let uniName = this.$store.getters.getUniversityName;
        if (uniName) {
            this.msg = uniName;
        } else {
            this.$store.dispatch("getUniversities", {term: ''}).then(({data}) => {
                let {universities} = data;
                this.uniSuggestList = universities;

            });
        }
    }
}