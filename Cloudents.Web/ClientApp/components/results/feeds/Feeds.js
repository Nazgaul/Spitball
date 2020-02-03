import feedSkeleton from '../../pages/feedPage/components/feedSkeleton/feedSkeleton.vue';
import feedFaqBlock from '../../pages/feedPage/components/feedFaqBlock/feedFaqBlock.vue';
import scrollList from '../../helpers/infinateScroll.vue';












import { LanguageService } from "../../../services/language/languageService";

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
                'Document':LanguageService.getValueByKey('feed_select_document'),
                'Video':LanguageService.getValueByKey('feed_select_video'),
                'Question':LanguageService.getValueByKey('feed_select_question'),
                'Tutor':LanguageService.getValueByKey('feed_select_tutor'),
                'Empty':LanguageService.getValueByKey('feed_select_all'),
            }
        };
    },
    computed: {
        ...mapGetters(['Feeds_getNextPageUrl','Feeds_getItems','Feeds_getFilters','Feeds_getCurrentQuery']),
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
        },
        fetchData(objParams){
            return this.Feeds_fetchingData(objParams)
        },
        handleSelects(){
            let objParams = {
                ...this.$route.query,
                ...this.query,
            }
            this.$router.push({name:'feed',query:{...objParams}})
        },
        getSelectedName(item){
            return this.dictionary[item.key]
            // console.log(e)
        }
    },
};