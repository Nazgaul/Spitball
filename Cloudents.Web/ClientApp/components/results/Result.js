import ResultItem from './ResultItem.vue';
import questionCard from './../question/helpers/question-card/question-card.vue';

const ResultTutor = () => import('./ResultTutor.vue');
const ResultBook = () => import('./ResultBook.vue');
const ResultJob = () => import('./ResultJob.vue');
import {page} from "../../services/navigation/vertical-navigation/nav";
import SuggestCard from './suggestCard.vue'
import emptyState from "./svg/no-match-icon.svg";
import {verticalsName} from "../../services/navigation/vertical-navigation/nav";
import {typesPersonalize} from "../settings/consts.js";
import signupBanner from './../helpers/signup-banner/signup-banner.vue'
import QuestionCard from "../question/helpers/question-card/question-card";
import {mapActions, mapGetters, mapMutations} from 'vuex'
import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
import loginToAnswer from '../question/helpers/loginToAnswer/login-answer.vue'

const ACADEMIC_VERTICALS = ['note', 'flashcard', 'book', 'tutor'];
import sortAndFilterMixin from '../mixins/sortAndFilterMixin'

import faqBlock from './helpers/faq-block/faq-block.vue'

import {skeletonData} from './consts'
import account from "../../store/account";

//The vue functionality for result page
export default {
    data() {
        return {
            pageData: '',
            selectedItem: null,
            filterObject: null,
            showFilters: false,
            showPersonalizeField: true,
            showFilterNotApplied: false,
            isLoad: false,
            showDialog: false
        };
    },

    components: {
        emptyState,
        ResultItem,
        SuggestCard,
        ResultTutor,
        ResultJob,
        ResultBook,
        questionCard,
        faqBlock,
        signupBanner,
        QuestionCard,
        sbDialog,
        loginToAnswer
    },

    //use basic sort and filter functionality( same for book details and result page)
    mixins: [sortAndFilterMixin],

    //when go back to home clear the saved term and classes
    beforeRouteLeave(to, from, next) {
        this.leavePage(to, from, next);
    },

    //When route has been updated(query,filter,vertical)
    beforeRouteUpdate(to, from, next) {
        this.updatePageData(to, from, next)
    },

    created() {
        //If query have courses save those courses
        if (this.query.course) this.setFilteredCourses(this.query.course);
        this.UPDATE_LOADING(true);
        //fetch data with the params
        this.fetchingData({
            name: this.name,
            params: {...this.query, ...this.params, term: this.userText}
        }).then((data) => {
                this.updateData.call(this, {...data, vertical: this.name})
            }).catch(reason => {
            console.error(reason);
            //when error from fetching data remove the loader
            this.UPDATE_LOADING(false);
        });
    },

    watch: {
        //update the course list of filters if have in page while the course list changes
        myCourses(val) {
            if (this.filterObject) {
                const courseIndex = this.filterObject.findIndex(i => i.modelId === "course");
                if (courseIndex > -1)
                    this.filterObject[courseIndex].data = val;
            }
        }
    },
    computed: {
        //get data from vuex getters
        ...mapGetters(['isFirst', 'myCourses', 'getFacet', 'getVerticalData', 'accountUser', 'showRegistrationBanner']),
        ...mapGetters({universityImage: 'getUniversityImage', university: 'getUniversity', items:'getSearchItems'}),
        
        filterCondition() {
            return this.filterSelection.length || (this.filterObject && this.page)
        },

        content: {
            get() {
                return this.pageData;
            },
            set(val) {
                if (val) {
                    this.pageData = val;
                    this.$nextTick(() => {
                        if (!this.items.length) {
                            // gaby: according to my understanding this code exists in order to notify
                            // google analytics that we have no questions in the page
                            Promise.resolve(() => {
                                let filters = {};
                                Object.entries(this.query).forEach(([key, currentVal]) => {
                                    if (key !== "sort" && key !== "q" && currentVal) {
                                        filters[key] = currentVal;
                                    }
                                });
                                return filters;
                            }).then(filters => {
                                let myFilters = filters();
                                let ExtraContent = "";
                                if (myFilters && Object.keys(myFilters).length) {
                                    ExtraContent = "#";
                                    Object.entries(myFilters).forEach(([key, currentVal]) => {
                                        ExtraContent += `${key}:[${currentVal}]`;
                                    });
                                    ExtraContent += "#";
                                }
                                this.$ga.event("Empty_State", this.name, ExtraContent + this.userText);
                            });
                        }
                        this.UPDATE_LOADING(false);
                        this.UPDATE_SEARCH_LOADING(false);
                    });
                }
            }
        },
        currentSuggest() {
            return verticalsName.filter(i => i !== this.name)[(Math.floor(Math.random() * (verticalsName.length - 2)))]
        },
        userText() {
            return this.query.q
        },
        isAcademic() {
            return ACADEMIC_VERTICALS.includes(this.name)
        },
        showSkelaton() {
            return this.loading || this.isLoad
        }
    },

    methods: {
        ...mapActions(['fetchingData', 'setFilteredCourses', 'cleanData', 'updateFacet', 'hideRegistrationBanner']),
        ...mapMutations(["UPDATE_SEARCH_LOADING"]),

        subFilterVertical(val) {
            return val.includes('note') || val === 'flashcard' || val === 'job' || val.includes('ask');
        },
        updatePageData(to, from, next) {
            (to.path === from.path && to.q === from.q) ? this.isLoad = true : this.UPDATE_LOADING(true);
            this.updateContentOfPage(to, from, next);
        },
        updateContentOfPage(to, from, next) {
            const toName = to.path.slice(1);
            const updateFilter = (to.path === from.path && to.query.q === from.query.q);
            this.fetchingData({name: toName, params: {...to.query, ...to.params, term: to.query.q}})
                .then((data) => {
                    //update data for this page
                    this.showFilterNotApplied = false;
                    this.updateData.call(this, {...data, vertical: toName}, updateFilter);
                    next();
                }).catch(reason => {
                //when error from fetching data remove the loader
                if (to.path === from.path && to.query.q === from.query.q) {
                    this.isLoad = false;
                    this.UPDATE_LOADING(false);
                    this.UPDATE_SEARCH_LOADING(false);
                    this.showFilterNotApplied = true;
                }
                else {
                    this.UPDATE_LOADING(false);
                    this.UPDATE_SEARCH_LOADING(false);
                    this.isLoad = false;
                    next();
                }
            });
        },
        leavePage(to, from, next) {
            if (to.name && to.name === 'home') {
                //clear boxes
                this.cleanData();
            }
            next();
        },
        //Function for update the filter object(when term or vertical change)
        $_updateFilterObject(vertical) {
            let currentPage = page[vertical];
            //validate current page have filters
            if (!currentPage || !currentPage.filter) {
                this.filterObject = null
            }
            else if (!this.subFilterVertical(vertical)) {
                this.filterObject = [{title: 'Status', modelId: "filter", data: currentPage.filter}];
            }
            else {
                //create filter object as the above structure but from list while the data is computed according to the filter id
                this.filterObject = currentPage.filter.map((i) => {
                    const item = {title: i.name, modelId: i.id};
                    item.data = (i.id === "course") ? this.myCourses : this.pageData[i.id] ? this.pageData[i.id] : this.getFacet ? this.getFacet : [];
                    return item;
                });
            }
        },
        updateData(data, isFilterUpdate = false) {
            const {facet} = data;
            facet ? this.updateFacet(facet) : this.updateFacet(null);
            this.pageData = {};
            this.content = data;
            //this.$emit('dataUpdated', this.items.length ? this.items[0] : null);
            this.filter = this.filterSelection;
            this.UPDATE_SEARCH_LOADING(false);
            (this.isLoad) ? this.isLoad = false : this.UPDATE_LOADING(false);
            if (this.isAcademic) {
                this.showPersonalizeField = true
            }
            //if the vertical or search term has been changed update the optional filters according
            if (!isFilterUpdate) {
                this.$_updateFilterObject(data.vertical);
            }
        },
        //functionality when remove filter from the selected filters
        $_removeFilter({value, key}) {
            this.UPDATE_SEARCH_LOADING(true);
            let updatedList = this.query[key];
            updatedList = [].concat(updatedList).filter(i => i.toString() !== value.toString());
            key === 'course' ? this.setFilteredCourses(updatedList) : "";
            this.$router.push({path: this.name, query: {...this.query, [key]: updatedList}});
        },
        //Open the personalize dialog when click on select course in class filter
        $_openPersonalize() {
            //emit event to open Login Dialog
            if (!this.accountUser) {
                this.$root.$emit("showLoginPopUp");

            }else {
                this.$root.$emit("personalize", typesPersonalize.course);
            }
        },
        //The presentation functionality for the selected filter(course=>take course name,known list=>take the terms from the const name,else=>the given name)
        $_showSelectedFilter({value, key}) {
            if (this.page && !this.subFilterVertical(this.name)) return this.page.filter.find(i => i.id === value).name;
            return key === 'course' && this.myCourses.find(x => x.id === Number(value)) ? this.myCourses.find(x => x.id === Number(value)).name : value;
        }
    },
};
