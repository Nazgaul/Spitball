import debounce from "lodash/debounce";
import {mapGetters, mapActions} from 'vuex'

export default {
    name: "uni-search-input",
    props: {
        placeholder: {type: String},
        prependIcon: {type: String, default:'sbf-search'},
    },
    data: () => ({
        msg: "",
        uniSuggestList: [],
        showSuggestions: false,
        focusedIndex: -1,
        originalMsg: ''
    }),
    computed: {
        ...mapGetters(['getUniversityName']),
        suggestList() {
            return this.uniSuggestList.slice(0, 3);
        },
    },
    watch: {
        msg: debounce(function (val) {
            if(this.focusedIndex >= 0 && this.msg !== this.suggestList[this.focusedIndex].name){
                this.focusedIndex = -1;
            }
                if (this.focusedIndex < 0) {
                    this.$emit('input', val);
                    this.$store.dispatch("getUniversities", {term: val}).then(({data}) => {
                        this.uniSuggestList = data["universities"];
                    });
                }
        }, 250),
        focusedIndex(val){
            if(val < 0){
                this.msg = this.originalMsg;
            }
            else {
                this.msg = this.suggestList[this.focusedIndex].name;
            }
        }
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
            let term = this.msg.toLowerCase();
            let itemLower = item.name.toLowerCase();
            let matchStartIndex = itemLower.indexOf(term);
            if (matchStartIndex < 0) {
                return item.name;
            }
            let matchEndIndex = matchStartIndex + term.length;
            return item.name.slice(0, matchStartIndex)
                + '<span class=\'highlight\'>' + item.name.slice(matchStartIndex, matchEndIndex) + '</span>'
                + item.name.slice(matchEndIndex, item.name.length);

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
        let uniName = this.$store.getters.getUniversityName;
        if (uniName) {
            this.msg = uniName;
        } else {
            this.$store.dispatch("getUniversities", {term: ''}).then(({data}) => {
                this.uniSuggestList = data["universities"];
            });
        }
    }
}