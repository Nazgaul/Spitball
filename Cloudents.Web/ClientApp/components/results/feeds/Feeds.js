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
import { mapActions, mapGetters } from 'vuex';
import resultFilter from '../helpers/resultFilter/resultFilter.vue';
import requestBox from '../../pages/feedPage/components/requestActions/requestActions.vue';
import coursesTab from "../../pages/feedPage/components/coursesTab/coursesTab.vue";
import generalPage from '../../helpers/generalPage.vue';

// SVG
import emptyState from "../svg/no-match-icon.svg";

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
    data() {
        return {
            showAdBlock: global.country === 'IL',
            query:{
                filter:this.$route.query.filter,
            },
            scrollBehaviour:{
                isLoading: false,
                isComplete: false,
                page: 1
            },
            dictionary:{
                'Document':this.$t('feed_select_document'),
                'Video':this.$t('feed_select_video'),
                'Question':this.$t('feed_select_question'),
                'Tutor':this.$t('feed_select_tutor'),
                'Empty':this.$t('feed_select_all'),
            }
        };
    },
    computed: {
        ...mapGetters(['Feeds_getItems','Feeds_getFilters','Feeds_getCurrentQuery']),
        items(){
            return this.Feeds_getItems
        },
        filters(){
            return this.Feeds_getFilters;
        },
    },
    watch: {
        Feeds_getCurrentQuery:{
            immediate:true,
            handler(newVal,oldVal){
                this.scrollBehaviour.page = 1;
                this.query.filter = this.Feeds_getCurrentQuery.filter
                if(JSON.stringify(newVal) !== JSON.stringify(oldVal)){
                    this.scrollBehaviour.isComplete = true;
                    this.fetchData({params:newVal}).finally(()=>{
                        this.scrollBehaviour.isLoading = false;
                        this.scrollBehaviour.isComplete = false;
                    });
                }
            }
        }
    },
    methods: {
        ...mapActions(['Feeds_fetchingData','Feeds_nextPage']),
        scrollFunc(){
            this.scrollBehaviour.isLoading = true;
            let nextPageQuery = {...this.$route.query,page: this.scrollBehaviour.page}
            let nextPageUrl = 'api/feed?'+ Object.keys(nextPageQuery).map(key => key + '=' + nextPageQuery[key]).join('&')
            if(!nextPageUrl) return this.scrollBehaviour.isLoading = false;

            this.Feeds_nextPage({url: nextPageUrl}).then((res) => {
                if (res.data && res.data.length) {
                    this.scrollBehaviour.isLoading = false;
                } else {
                    this.scrollBehaviour.isComplete = true;
                }
                this.scrollBehaviour.page++
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
        },
        fetchData(objParams){
            return this.Feeds_fetchingData(objParams)
        },
        handleSelects(){
            let objParams = {
                ...this.$route.query,
                ...this.query,
            }
            Object.keys(objParams).forEach((key) => (objParams[key] === '') && delete objParams[key]);
            this.$router.push({name:'feed',query:{...objParams}})
            this.scrollBehaviour.page = 1;
        },
        getSelectedName(item){
            return this.dictionary[item.key]
        }
    },
};