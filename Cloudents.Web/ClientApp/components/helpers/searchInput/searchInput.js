import debounce from "lodash/debounce";
import { mapActions, mapGetters, mapMutations } from 'vuex';
import * as consts from '../consts';
import classIcon from "./img/search-class-icon.svg";
import universityIcon from "./img/search-university-icon.svg";
import spitballIcon from "./img/search-spitball-icon.svg";
import { LanguageService } from "../../../services/language/languageService";

export default {
    name: "search-input",
    components: {
        classIcon,
        spitballIcon,
        universityIcon
    },
    props: {
        hideOnScroll: {
            type: Boolean,
            default: false
        },
        placeholder: {
            type: String
        },
        userText: {
            String
        },
        submitRoute: {
            String,
            default: '/ask'
        },
        suggestionVertical: {
            String
        },
    },
    data: () => ({
        autoSuggestList: [],
        isFirst: true,
        showSuggestions: false,
        focusedIndex: -1,
        originalMsg: '',
        isRtl: global.isRtl,
        suggestOptions: [
            {
                name: LanguageService.getValueByKey("searchInput_class_search"),
                icon: "classIcon",
                id: 1
            },
            {
                name: LanguageService.getValueByKey("searchInput_university_search"),
                icon: "universityIcon",
                id: 2
            },
            {
                name: LanguageService.getValueByKey("searchInput_spitball_search"),
                icon: "spitballIcon",
                id: 3
            },
        ]
    }),
    computed: {
        ...mapGetters({
                          'globalTerm': 'currentText'
                      }),
        ...mapGetters(['allHistorySet', 'getCurrentVertical', 'getVerticalHistory']),
        isSearchActive() {
            if(this.$vuetify.breakpoint.xsOnly) {
                return !!this.$route.query && !!this.$route.query.term;
            } else {
                return false;
            }

        },
        suggestList() {
            let dynamicSuggest = [];
            if(this.courseSelected()) {
                dynamicSuggest = [].concat(this.suggestOptions);
            } else {
                dynamicSuggest = this.suggestOptions.filter(searchOption => {
                    return searchOption.id !== 1;
                });
            }
            return dynamicSuggest;
        },

        isHome() {
            return this.$route.name === 'home';
        },
        maxResults() {
            return this.isHome ? consts.HOME_MAX_SUGGEST_NUM : consts.VERTICAL_MAX_SUGGEST_NUM;
        },
    },
    watch: {
        userText(val) {
            this.msg = val;
            this.isFirst = true;
        },
        focusedIndex(val) {
            if(val < 0) {
                this.msg = this.originalMsg;
            } else {
                this.msg = this.suggestList[this.focusedIndex].text;
            }
        },
        '$route'() {
            this.$forceUpdate();
        },
    },
    methods: {
        ...mapActions(['getAutocmplete', 'changeSelectPopUpUniState', 'setUniversityPopStorage_session']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),
        ...mapGetters(['getUniversityPopStorage', 'accountUser', 'getSchoolName']),
        goBackFromSearch() {
            this.msg = "";
            this.search();
        },
        outsideClick(event) {
            console.log('clicked outside');
            this.closeSuggestions();
        },
        selectos(item) {
            //this.msg = item.text;
            //this.$ga.event('Search_suggestions', `Suggest_${this.getCurrentVertical ?
            // this.getCurrentVertical.toUpperCase() : 'HOME'}_${item.type}`, `#${index + 1}_${item}`);
            this.search(item.id);
            this.closeSuggestions();
        },
        courseSelected() {
            return !!this.$route.query ? !!this.$route.query.Course : false;
        },
        search(id) {
            //prevent search empty string when no term is in search ATM
            if(!this.$route.query.term) {
                if(!this.msg) {
                    return;
                }
            }
            if(this.msg === this.$route.query.term){
                return
            }
            this.UPDATE_SEARCH_LOADING(true);
            let query = this.prepareQuery(id);
            this.$router.push({
                                  path: this.submitRoute,
                                  query
                              });

            this.closeSuggestions();
            // to remove keyboard on mobile
            this.$nextTick(() => {
                this.$el.querySelector('input').blur();
            });
        },
        prepareQuery(typeId) {
            let query = {
                term: this.msg
            };
            if(!this.msg) {
                delete query.term;
            }
            if(typeId === 1) {
                let course = this.$route.query.Course;
                let uni = this.getSchoolName();
                query.Course = course;
                query.uni = uni;
            } else if(typeId === 2) {
                let uni = this.getSchoolName();
                query.uni = uni;
            } else if(typeId === 3) {
                //query should incude only term
            } else {
                if(!!this.$route.query.Course) {
                    let course = this.$route.query.Course;
                    query.Course = course;
                }
            }
            return query;
        },
        openUniPop() {
            console.log("uni select pop");
            this.changeSelectPopUpUniState(true);

        },
        showUniPop() {
            let user = this.accountUser();
            if(!!user && !user.universityExists) {
                console.log("select uni pop up check");
                let uniStoragePop = this.getUniversityPopStorage();
                if(uniStoragePop.local < 3 && !uniStoragePop.session) {
                    this.openUniPop();
                    this.setUniversityPopStorage_session();
                    return true;
                }
                return false;
            } else {
                return false;
            }
        },
        openSuggestions() {
            //if user with no university pop it, up to 3 times in seperated seassons
            if(this.showUniPop()) {
                return;
            }
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
            this.focusedIndex = -1;
            //this.msg = this.originalMsg;
            if(this.showSuggestions) {
                this.showSuggestions = false;
                //this.$el.querySelector('.search-menu').scrollTop = 0;
            }
        }
    },
    created() {
        if(!this.isHome) {
            this.msg = this.userText ? this.userText : this.globalTerm ? this.globalTerm : "";
        }
    }

};