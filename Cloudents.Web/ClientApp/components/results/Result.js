﻿import ResultItem from './ResultItem.vue';
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
//update data function update the page content and selected filters
let updateData = function (data, isFilterUpdate = false) {
    const {facet} = data;
    facet ? this.updateFacet(facet) : this.updateFacet(null);
    this.pageData = {};
    this.content = data;
    this.$emit('dataUpdated', this.items.length ? this.items[0] : null);
    //this.$emit('dataUpdated', data.data.length ? data.data[0] : null);
    // (data.data.length && this.hasExtra) ? this.selectedItem = data.data[0].placeId : '';
    this.filter = this.filterSelection;
    // this.UPDATE_LOADING(false);
    this.UPDATE_SEARCH_LOADING(false);
    (this.isLoad) ? this.isLoad = false : this.UPDATE_LOADING(false);
    if (this.isAcademic) {
        this.showPersonalizeField = true
    }

    //if the vertical or search term has been changed update the optional filters according
    if (!isFilterUpdate) {
        this.$_updateFilterObject(data.vertical);
    }
};
//The vue functionality for result page
export default {
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
        // ...mapGetters({universityImage: 'getUniversityImage', university: 'getUniversity'}),
        ...mapGetters({universityImage: 'getUniversityImage', university: 'getUniversity', items:'getSearchItems'}),
        
        currentPromotion() {
            return promotions[this.name]
        },
        filterCondition() {
            return this.filterSelection.length || (this.filterObject && this.page)
            // return this.filterSelection.length || (this.filterObject && this.page && this.items.length)
        },
        content: {
            get() {
                return this.pageData;
            },
            set(val) {
                if (val) {
                    this.pageData = val;
                    //this.updateItems(val.data);
                    // this.items = val.data;
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
        isEmpty: function () {
            return this.pageData.data ? !this.pageData.data.length : true
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

    data() {
        return {
            //items: '',
            pageData: '',
            selectedItem: null,
            filterObject: null,
            showFilters: false,
            showPersonalizeField: true,
            //showPromo: this.isPromo,
            showFilterNotApplied: false,
            isLoad: false,
            offsetTop: 0,
            isBannerVisible: true,
            showDialog: false

            // accountUser: {}
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

    created() {
        
        //If query have courses save those courses
        if (this.query.course) this.setFilteredCourses(this.query.course);
        this.UPDATE_LOADING(true);
        // this.items = skeletonData[this.name];
        //this.updateItems(skeletonData[this.name])
        //fetch data with the params
        this.fetchingData({
            name: this.name,
            params: {...this.query, ...this.params, term: this.userText}
        })
            .then((data) => {
                updateData.call(this, {...data, vertical: this.name});//irena
            }).catch(reason => {
            console.error(reason);
            //when error from fetching data remove the loader
            this.UPDATE_LOADING(false);
            //this.updateItems([]);
            // this.items = [];
        });
        // });
    },
    methods: {
        //...mapMutations({updateItems: "UPDATE_ITEMS"}),
        ...mapMutations(["UPDATE_SEARCH_LOADING"]),
        subFilterVertical(val) {
            return val.includes('note') || val === 'flashcard' || val === 'job' || val.includes('ask');
        },
        //for skeleton
        updatePageData(to, from, next) {
            (to.path === from.path && to.q === from.q) ? this.isLoad = true : this.UPDATE_LOADING(true);
            const toName = to.path.slice(1);
            const itemsBeforeUpdate = this.items;
            this.pageData = {};
            
            //this.updateItems(skeletonData[this.name])
            // this.items = [];
            // this.items = skeletonData[toName];
            this.updateContentOfPage(to, from, next, itemsBeforeUpdate);
        },
        updateOnScroll(value){
            //this.updateItems(this.items.concat(value))
            // this.items=this.items.concat(value)
        },
        updateContentOfPage(to, from, next, itemsBeforeUpdate) {
            const toName = to.path.slice(1);
            const updateFilter = (to.path === from.path && to.query.q === from.query.q);
            this.fetchingData({name: toName, params: {...to.query, ...to.params, term: to.query.q}})
                .then((data) => {
                    //update data for this page
                    this.showFilterNotApplied = false;
                    updateData.call(this, {...data, vertical: toName}, updateFilter);
                    window.scrollTo(0, 0);
                    next();
                }).catch(reason => {
                window.scrollTo(0, 0)
                //when error from fetching data remove the loader
                if (to.path === from.path && to.query.q === from.query.q) {
                    this.isLoad = false;
                    this.UPDATE_LOADING(false);
                    this.UPDATE_SEARCH_LOADING(false);
                    this.showFilterNotApplied = true;
                    //this.updateItems(itemsBeforeUpdate)
                    // this.items = itemsBeforeUpdate;
                }
                else {
                    this.UPDATE_LOADING(false);
                    this.UPDATE_SEARCH_LOADING(false);
                    this.isLoad = false;
                    //this.updateItems([])
                    // this.items = [];
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
        }
        ,
//Get functions from vuex actions
        ...mapActions(['fetchingData', 'setFilteredCourses', 'cleanData', 'updateFacet', 'hideRegistrationBanner']),
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
        }
        ,

//functionality when remove filter from the selected filters
        $_removeFilter({value, key}) {
            this.UPDATE_SEARCH_LOADING(true);
            let updatedList = this.query[key];
            updatedList = [].concat(updatedList).filter(i => i.toString() !== value.toString());
            key === 'course' ? this.setFilteredCourses(updatedList) : "";
            this.$router.push({path: this.name, query: {...this.query, [key]: updatedList}});
        }
        ,
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
//Page props come from the route
    //props: {
    //    isPromo: {
    //        type: Boolean
    //    }
    //}
};
