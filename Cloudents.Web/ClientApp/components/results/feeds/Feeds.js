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
            showAdBlock: global.country === 'IL',
            pageData: '',
            scrollBehaviour:{
                isLoading: false,
                isComplete: false,
                page: 1
            }
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
    },
    methods: {
        ...mapActions([
            'Feeds_fetchingData',
            'updateLoginDialogState',
            'updateNewQuestionDialogState',
            'Feeds_nextPage',

        ]),
        goToAskQuestion(){
            (this.accountUser == null)? this.updateLoginDialogState(true) : this.updateNewQuestionDialogState(true);
        },
        scrollFunc(){
            this.scrollBehaviour.isLoading = true;
            let nextPageUrl = this.Feeds_getNextPageUrl;

            if(!nextPageUrl) return this.scrollBehaviour.isLoading = false;

            this.Feeds_nextPage({url: nextPageUrl}).then((res) => {
                if (res.data && res.data.length) {
                    this.scrollBehaviour.isLoading = false;
                } else {
                    this.scrollBehaviour.isComplete = true;
                }
            }).catch(() => {
                this.scrollBehaviour.isComplete = true;
            });
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
        let objParams = {
            params: {...this.query, ...this.params, term: this.userText},
        }
        this.Feeds_fetchingData(objParams)
    },

    beforeRouteUpdate(to, from, next) {
        this.scrollBehaviour.isComplete = true;
        let params = {...to.query, ...to.params, term: to.query.term};
        //TODO check about scrolling?? 
        this.Feeds_fetchingData({params}).finally(()=>{
            this.scrollBehaviour.isLoading = false;
            this.scrollBehaviour.isComplete = false;
            next();
        });
    },
};