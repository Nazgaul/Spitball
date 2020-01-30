import feedSkeleton from '../../pages/feedPage/components/feedSkeleton/feedSkeleton.vue';
import feedFaqBlock from '../../pages/feedPage/components/feedFaqBlock/feedFaqBlock.vue';

import scrollList from '../../helpers/infinateScroll.vue';
// cards:
import tutorResultCardMobile from '../tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import resultItem from '../ResultItem.vue';
import resultAsk from "../ResultAsk.vue";
import resultNote from "../ResultNote.vue";
import tutorResultCard from '../tutorCards/tutorResultCard/tutorResultCard.vue';
import suggestCard from '../suggestCard.vue';
import emptyStateCard from '../emptyStateCard/emptyStateCard.vue';

















import { mapActions, mapGetters, mapMutations } from 'vuex';
// SERVICES
import analyticsService from '../../../services/analytics.service';
// COMPONENTS 


import resultFilter from '../helpers/resultFilter/resultFilter.vue';

import requestBox from '../../pages/feedPage/components/requestActions/requestActions.vue';
import coursesTab from "../../pages/feedPage/components/coursesTab/coursesTab.vue";
import generalPage from '../../helpers/generalPage.vue';

// SVG
import emptyState from "../svg/no-match-icon.svg";
// STORE
import storeService from "../../../services/store/storeService";
import feedStore from '../../../store/feedStore';

export default {
    components: {
        feedSkeleton,
        feedFaqBlock,



        scrollList,
        tutorResultCardMobile,
        ResultItem: resultItem,
        ResultAsk: resultAsk,
        ResultNote: resultNote,
        tutorResultCard,
        SuggestCard: suggestCard,



        emptyState,
        resultFilter,
        emptyStateCard,
        requestBox,
        coursesTab,
        generalPage
    },
    props:{
        params: {type: Object},
        name: {type: String},
        query: {type: Object},
    },
    data() {
        return {
            pageData: '',
            isLoad: false,
            scrollBehaviour:{
                isLoading: false,
                isComplete: false,
                page: 1
            },






            feedGlobalProps:{
                scrollFunc: this.scrollFunc,
                scrollBehaviour: this.getScrollBehaviour,
                openRequestTutor: this.openRequestTutor,
                goToAskQuestion: this.goToAskQuestion,
                userText: this.userText,
            },
        };
    },
    computed: {
        ...mapGetters([
            'accountUser', 
            'Feeds_getNextPageUrl',
            'Feeds_getItems'
        ]),
        items(){
            return this.Feeds_getItems
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
            'updateLoginDialogState',
            'updateNewQuestionDialogState',
            'Feeds_nextPage',
            'analyticsService',
            'updateRequestDialog',
            'setTutorRequestAnalyticsOpenedFrom'
        ]),
        ...mapMutations(["UPDATE_SEARCH_LOADING",'UPDATE_LOADING']),


        getScrollBehaviour(){
            return this.scrollBehaviour;
        },


        goToAskQuestion(){
             if(this.accountUser == null){
                this.updateLoginDialogState(true);
            }else{
                 this.updateNewQuestionDialogState(true);
            }
        },
        scrollFunc(){
            this.scrollBehaviour.isLoading = true;
            let nextPageUrl = this.Feeds_getNextPageUrl;

            if(!nextPageUrl) return this.scrollBehaviour.isLoading = false;

            this.Feeds_nextPage({url: nextPageUrl})
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
            if(to.path === from.path && to.q === from.q){
                this.isLoad = true
            }else{
                this.UPDATE_LOADING(true);
            }
            this.updateContentOfPage(to, from, next);
        },
        //    3-%%%   fetching data and calling updateData func
        updateContentOfPage(to, from, next) {
            this.scrollBehaviour.isComplete = true;
            const toName = to.path.slice(1);
            let params=  {...to.query, ...to.params, term: to.query.term};
            this.Feeds_fetchingData({name: toName, params}, true)
                .then((data) => {
                    this.pageData = data;
                    next();
                }).catch((err) => {
                    console.log(err)
                    next();
            }).finally(()=>{
                //error handler

                this.UPDATE_SEARCH_LOADING(false);
                this.UPDATE_LOADING(false);
                this.isLoad = false;
                //scroll handler
                this.scrollBehaviour.isLoading = false;
                this.scrollBehaviour.isComplete = false;
            });
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
        storeService.lazyRegisterModule(this.$store, 'feeds', feedStore);
        
        this.UPDATE_LOADING(true);
        let objParams = {
            name: this.name,
            params: {...this.query, ...this.params, term: this.userText},
            skipLoad: this.$route.path.indexOf("question") > -1
        }
        this.Feeds_fetchingData(objParams).then((data) => {  
            this.pageData = data;          
            }).catch(reason => {
                console.error(reason);
            }).finally(()=>{
                this.UPDATE_LOADING(false);
                this.UPDATE_SEARCH_LOADING(false);
            });
    },







    // INFO:  #1 When route has been updated(query,filter,course)
    beforeRouteUpdate(to, from, next) {
        this.updatePageData(to, from, next);
    },
};
