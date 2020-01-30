import feedSkeleton from '../../pages/feedPage/components/feedSkeleton/feedSkeleton.vue';
import feedFaqBlock from '../../pages/feedPage/components/feedFaqBlock/feedFaqBlock.vue';
import feedItemList from '../../pages/feedPage/components/feedItemList/feedItemList.vue';


















import { mapActions, mapGetters, mapMutations } from 'vuex';
// SERVICES
import { verticalsName } from "../../../services/navigation/vertical-navigation/nav";
import analyticsService from '../../../services/analytics.service';
// COMPONENTS 
import resultItem from '../ResultItem.vue';
import resultAsk from "../ResultAsk.vue";
import resultNote from "../ResultNote.vue";
import suggestCard from '../suggestCard.vue';
import resultFilter from '../helpers/resultFilter/resultFilter.vue';
import emptyStateCard from '../emptyStateCard/emptyStateCard.vue';

import requestBox from '../../pages/feedPage/components/requestActions/requestActions.vue';
import tutorResultCard from '../tutorCards/tutorResultCard/tutorResultCard.vue';
import tutorResultCardMobile from '../tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import coursesTab from "../../pages/feedPage/components/coursesTab/coursesTab.vue";
import generalPage from '../../helpers/generalPage.vue';

// SVG
import emptyState from "../svg/no-match-icon.svg";
// MIXIN
import sortAndFilterMixin from '../../mixins/sortAndFilterMixin';
// STORE
import storeService from "../../../services/store/storeService";
import feedStore from '../../../store/feedStore';

export default {
    components: {
        feedSkeleton,
        feedItemList,
        feedFaqBlock,






        emptyState,
        SuggestCard: suggestCard,
        ResultAsk: resultAsk,
        ResultNote: resultNote,
        ResultItem: resultItem,
        resultFilter,
        emptyStateCard,
        requestBox,
        tutorResultCard,
        tutorResultCardMobile,
        coursesTab,
        generalPage
    },
    data() {
        return {
            pageData: '',
            filterObject: null,
            showFilterNotApplied: false,
            isLoad: false,
            scrollBehaviour:{
                isLoading: false,
                isComplete: false,
                page: 1
            },
        };
    },
    //use basic sort and filter functionality( same for book details and result page)
    mixins: [sortAndFilterMixin],



    computed: {
        ...mapGetters([
            'getFilters', 
            'accountUser', 
            'Feeds_getNextPageUrl'
        ]),
        ...mapGetters({university: 'getUniversity', items:'Feeds_getItems'}),

        // ( filterObject && this.page);

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
                                    let extraContent = "";
                                    if (myFilters && Object.keys(myFilters).length) {
                                        extraContent = "#";
                                        Object.entries(myFilters).forEach(([key, currentVal]) => {
                                            extraContent += `${key}:[${currentVal}]`;
                                        });
                                        extraContent += "#";
                                    }
                                    this.$ga.event("Empty_State", this.name, extraContent + this.userText);
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
        userText() {
            return this.query.term;
        },  
        showAdBlock() {
            return global.country === 'IL';
        }
    },
    methods: {
        ...mapActions([
            'Feeds_fetchingData',
            'setFilteredCourses',
            'updateLoginDialogState',
            'updateNewQuestionDialogState',
            'Feeds_nextPage',
            'analyticsService',
            'updateRequestDialog',
            'setTutorRequestAnalyticsOpenedFrom'
        ]),
        ...mapMutations(["UPDATE_SEARCH_LOADING",'UPDATE_LOADING']),
        goToAskQuestion(){
             if(this.accountUser == null){
                this.updateLoginDialogState(true);
            }else{
                //ab test original do not delete
                 this.updateNewQuestionDialogState(true);
            }
        },
        scrollFunc(){
            this.scrollBehaviour.isLoading = true;
            let nextPageUrl = this.Feeds_getNextPageUrl;

            if(!nextPageUrl) return this.scrollBehaviour.isLoading = false;

            this.Feeds_nextPage({vertical: this.pageData.vertical, url: nextPageUrl})
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
            let params=  {...to.query, ...to.params, term: to.query.term};
            this.Feeds_fetchingData({name: toName, params}, true)
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
            }).finally(()=>{
                //error handler
                this.UPDATE_SEARCH_LOADING(false);
                this.isLoad = false;
                this.UPDATE_LOADING(false);
                //scroll handler
                this.scrollBehaviour.isLoading = false;
                this.scrollBehaviour.isComplete = false;
            });
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
            //if the vertical or search term has been changed update the filters according
            if (!isFilterUpdate) {
                this.$_updateFilterObject();
            }
        },

        openRequestTutor() {
            analyticsService.sb_unitedEvent('Tutor_Engagement', 'request_box');
            this.setTutorRequestAnalyticsOpenedFrom({
                component: 'suggestCard',
                path: this.$route.path
            });
            this.updateRequestDialog(true);
        },
        setTemplate(template) {
            if(template === 'tutor-result-card') {
                if(this.$vuetify.breakpoint.xsOnly) {
                    return 'tutor-result-card-mobile';
                }
            } else {
                if(template === 'item') {
                    return 'result-item';
                }
            }
            return template;
        }
    },
    created() {
        //register Feeds Store
        storeService.lazyRegisterModule(this.$store, 'feeds', feedStore);
        //If query have courses save those courses
        // if (this.query.course) this.setFilteredCourses(this.query.course);
        
        this.UPDATE_LOADING(true);
        //fetch data with the params
        this.Feeds_fetchingData({
            name: this.name,
            params: {...this.query, ...this.params, term: this.userText},
            skipLoad: this.$route.path.indexOf("question") > -1
        }).then((data) => {            
            this.updateData.call(this, {...data, vertical: this.name});
        }).catch(reason => {
            console.error(reason);
            this.UPDATE_SEARCH_LOADING(false);
        });
    },







    // INFO:  #1 When route has been updated(query,filter,course)
    beforeRouteUpdate(to, from, next) {
        this.updatePageData(to, from, next);
    },
};
