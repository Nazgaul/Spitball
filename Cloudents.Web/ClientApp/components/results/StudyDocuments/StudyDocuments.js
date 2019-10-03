// V14 do we still need this ?
import ResultItem from '../ResultItem.vue'; //
import ResultNote from "../ResultNote.vue";
import { verticalsName, verticalsNavbar } from "../../../services/navigation/vertical-navigation/nav";
import SuggestCard from '../suggestCard.vue'
import emptyState from "../svg/no-match-icon.svg";
import { mapActions, mapGetters, mapMutations } from 'vuex'
import sbDialog from '../../wrappers/sb-dialog/sb-dialog.vue';
import loginToAnswer from '../../question/helpers/loginToAnswer/login-answer.vue';
import sortAndFilterMixin from '../../mixins/sortAndFilterMixin';
import { LanguageService } from '../../../services/language/languageService'

import setUniClass from '../helpers/setUniClassItem/setUniClass.vue'
import analyticsService from '../../../services/analytics.service'


import faqBlock from '../helpers/faq-block/faq-block.vue'

import askQuestionBtn from '../helpers/askQuestionBtn/askQuestionBtn.vue'
import uploadFilesBtn from "../helpers/uploadFilesBtn/uploadFilesBtn.vue"
import schoolBlock from '../../schoolBlock/schoolBlock.vue'
import resultFilter from '../helpers/resultFilter/resultFilter.vue'
import emptyStateCard from '../emptyStateCard/emptyStateCard.vue'
import requestBox from '../../requestActions/requestActions.vue'


//store
import storeService from '../../../services/store/storeService';
import studyDocumentsStore from '../../../store/studyDocuments_store';

//The vue functionality for result page
export default {
    components: {
        emptyState,
        ResultItem,
        ResultNote,
        SuggestCard,
        faqBlock,
        sbDialog,
        loginToAnswer,
        uploadFilesBtn,
        askQuestionBtn,
        
        setUniClass,
        schoolBlock,
        resultFilter,
        emptyStateCard,
        requestBox
    },
    data() {
        return {
            pageData: '',
            selectedItem: null,
            filterObject: null,
            showFilters: false,
            showPersonalizeField: true,
            showFilterNotApplied: false,
            isLoad: false,
            showDialog: false,
            placeholder: {
                whereSchool: LanguageService.getValueByKey("result_where_school")
            },
            scrollBehaviour: {
                isLoading: false,
                isComplete: false,
                page: 1
            }
        };
    },


    //use basic sort and filter functionality( same for book details and result page)
    mixins: [sortAndFilterMixin],
    //when trying to go back to '/'
    beforeRouteLeave(to, from, next) {
        if (to.name && to.name === 'home') {
            //clear filters boxes
            this.cleanData();
        }
        next();
    },
    //When route has been updated(query,filter,vertical) 1-%%%
    beforeRouteUpdate(to, from, next) {
        this.updatePageData(to, from, next);

    },

    computed: {
        //get data from vuex getters
        ...mapGetters(['isFirst', 'myCourses', 'getDialogState', 'getFilters', 'accountUser', 'getSchoolName', 'getReflectChangeToPage', 'getSearchLoading']),
        ...mapGetters({
            universityImage: 'getUniversityImage',
            university: 'getUniversity',
            items: 'StudyDocuments_getItems'
        }),
        showSelectUni() {
            let schoolName = this.getSchoolName;
            return schoolName.length === 0;
        },
        isNote() {
            return this.$route.path.slice(1) === 'note';
        },
        //not interesting
        filterCondition() {
            return this.filterSelection.length || (this.filterObject && this.page);
        },
        content: {
            get() {
                return this.pageData;
            },
            set(val) {
                if (val) {
                    this.pageData = val;
                    this.$nextTick(() => {
                        if (this.items !== undefined && this.items !== null && !this.items.length) {
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
                            })
                                .then(filters => {
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
            return verticalsName.filter(i => i !== this.name)[(Math.floor(Math.random() * (verticalsName.length - 2)))];
        },
        currentNavData() {
            return verticalsNavbar.filter((navItem) => {
                return navItem.id === this.name;
            })[0];
        },
        userText() {
            return this.query.term;
        },
        //isAcademic() {
        //    return ACADEMIC_VERTICALS.includes(this.name);
        //},
        showSkeleton() {
            return this.getSearchLoading || this.loading || this.isLoad;
        }
    },

    watch: {
        getSchoolName() {
            console.log("school name changed");
            if (this.getResultLockForSchoolNameChange()) {
                this.reloadContentOfPage();
            }
        },
        getReflectChangeToPage() {
            if (this.getResultLockForClassesChange()) {
                this.reloadContentOfPage();
            }
        }
    },
    methods: {
        ...mapActions([
            'StudyDocuments_fetchingData',
            'setFilteredCourses',
            'cleanData',
            'updateFilters',
            'updateLoginDialogState',
            'updateUserProfileData',
            'updateNewQuestionDialogState',
            'updateDialogState',
            'StudyDocuments_nextPage',
            'updateRequestDialog',
            'setTutorRequestAnalyticsOpenedFrom'
        ]),
        ...mapMutations(["UPDATE_SEARCH_LOADING"]),
        ...mapGetters(["StudyDocuments_getNextPageUrl", "getResultLockForSchoolNameChange", "getResultLockForClassesChange"]),
        goToAskQuestion() {
            if (this.accountUser == null) {
                this.updateLoginDialogState(true);
                //user profile update
                this.updateUserProfileData('profileHWH');
            } else {
                //ab test original do not delete
                this.updateNewQuestionDialogState(true);
            }
        },
        scrollFunc() {
            this.scrollBehaviour.isLoading = true;
            let nextPageUrl = this.StudyDocuments_getNextPageUrl();
            if (this.name !== this.pageData.vertical) {
                this.scrollBehaviour.isLoading = false;
                return
            };
            this.StudyDocuments_nextPage({vertical: this.pageData.vertical, url: nextPageUrl})
                .then((res) => {
                    if (res.data && res.data.length) {
                        this.scrollBehaviour.isLoading = false;
                    } else {
                        this.scrollBehaviour.isComplete = true;
                    }
                }).catch(() => {
                this.scrollBehaviour.isComplete = true;
            });
        },
        //   2-%%%
        updatePageData(to, from, next) {
            (to.path === from.path && to.q === from.q) ? this.isLoad = true : this.UPDATE_LOADING(true);
            this.updateContentOfPage(to, from, next);
        },
        //    3-%%%   fetching data and calling updateData func
        updateContentOfPage(to, from, next) {
            this.scrollBehaviour.isComplete = true;
            const toName = to.path.slice(1);
            let params = {...to.query, ...to.params, term: to.query.term};
            this.StudyDocuments_fetchingData({name: toName, params}, true)
                .then((data) => {
                    //update data for this page
                    this.showFilterNotApplied = false;
                    this.updateData.call(this, {...data, vertical: toName});
                    next();
                }).catch(() => {
                //when error from fetching data remove the loader
                if (to.path === from.path && to.query.term === from.query.term) {
                    this.showFilterNotApplied = true;
                }
                else {
                    next();
                }
            }).finally(() => {
                //error handler
                this.UPDATE_SEARCH_LOADING(false);
                this.isLoad = false;
                this.UPDATE_LOADING(false);
                //scroll handler
                this.scrollBehaviour.isLoading = false;
                this.scrollBehaviour.isComplete = false;
            });
        },
        reloadContentOfPage() {
            let noop = function () {
            };
            let to = this.$route;
            let from = this.$route;
            this.UPDATE_SEARCH_LOADING(true);
            this.updateContentOfPage(to, from, noop);
        },
        //Function for update the filter object(when term or vertical change)
        $_updateFilterObject() {
            this.filterObject = this.getFilters;
        },

        //   4-%%%
        updateData(data, isFilterUpdate = false) {
            this.pageData = {};
            this.content = data;
            this.filter = this.filterSelection;
            this.UPDATE_SEARCH_LOADING(false);
            (this.isLoad) ? this.isLoad = false : this.UPDATE_LOADING(false);
            if (this.isAcademic) {
                this.showPersonalizeField = true;
            }
            //if the vertical or search term has been changed update the filters according
            if (!isFilterUpdate) {
                this.$_updateFilterObject();
            }
        },
        //removes filter from selected filter
        $_removeFilter({filterId: value, filterType: key}) {
            this.UPDATE_SEARCH_LOADING(true);
            let updatedList = this.query[key];
            updatedList = [].concat(updatedList).filter(i => i.toString() !== value.toString());
            key === 'course' ? this.setFilteredCourses(updatedList) : "";
            this.$router.push({path: this.name, query: {...this.query, [key]: updatedList}});
        },

        //The presentation functionality for the selected filter(course=>take course name,known list=>take the terms from the const name,else=>the given name)
        $_showSelectedFilter({value, key}) {
            return value;
        },
        /*watchinNowStyle(item){
            let sameUser = false;
            let userId = this.accountUser ? this.accountUser.id : -1;
            if(!!item.user){
                sameUser = userId === item.user.id;
            }
            if(!item.color){
                //if item color is undefined set it to default so the color wont be white
                item.color = "default";
            }
        return {
            'color': item.color !== 'default' ? 'white' : '',
            //'bottom' : sameUser ? '15px' : ''
            }
        },*/
        openRequestTutor() {
            analyticsService.sb_unitedEvent('Tutor_Engagement', 'request_box');
            this.setTutorRequestAnalyticsOpenedFrom({
                component: 'suggestCard',
                path: this.$route.path
            });
            this.updateRequestDialog(true);
        }
    },
    beforeDestroy(){
    },
    created() {
        storeService.lazyRegisterModule(this.$store,'studyDocumentsStore',studyDocumentsStore);

        //If query have courses save those courses
        if (this.query.course) this.setFilteredCourses(this.query.course);
        this.UPDATE_LOADING(true);
        //fetch data with the params
        this.StudyDocuments_fetchingData({
            name: this.name,
            params: {...this.query, ...this.params, term: this.userText},
            skipLoad: this.$route.path.indexOf("question") > -1
        }).then((data) => {
            this.updateData.call(this, {...data, vertical: this.name});
        }).catch(reason => {
            console.error(reason);
            //when error from fetching data remove the loader
            this.UPDATE_LOADING(false);
        });
    }

};
