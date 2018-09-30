import questionCard from "../question/helpers/question-card/question-card.vue";
import userBlock from '../helpers/user-block/user-block.vue';
import {dollarCalculate} from "../../store/constants";
import accountService from '../../services/accountService';
import {mapGetters, mapActions} from 'vuex'
import { LanguageService } from "../../services/language/languageService";

export default {
    components: {questionCard, userBlock},
    props: {
        id: {Number}
    },
    data() {
        return {
            activeTab: 1,
            profileData: null,

        }
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState']),

        changeActiveTab(tabId) {
            this.activeTab = tabId;
            this.$router.meta = {previous : tabId}
        },
        fetchData() {
            accountService.getProfile(this.id).then(({ data }) => {
                this.profileData = data;
            }, error => {
                window.location = "/error/notfound";
            })
        }
    },
    computed: {
        ...mapGetters(["accountUser"]),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        myAnswers() {
            return this.profileData.answers ? this.profileData.answers.map(i => {
                return {
                    ...i,
                    //user: this.profileData.user,
                    answersNum: i.answers,
                    filesNum: i.files,
                }
            }) : []
        },
        questions() {
            return this.profileData.questions ? this.profileData.questions.map(item => {
                return {
                    ...item,
                    user: this.profileData.user,
                    answersNum: item.answers,
                    filesNum: item.files,
                }
            }) : []
        },
        isMyProfile() {
            return this.accountUser && this.accountUser.id && this.profileData ? this.profileData.user.id == this.accountUser.id : false;
        },
        emptyStateData() {
            var questions = {
                text: LanguageService.getValueByKey("profile_emptyState_questions_text"),
                boldText: LanguageService.getValueByKey("profile_emptyState_questions_boldText"),
                btnText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                btnUrl:  ()=> {
                    this.updateNewQuestionDialogState(true)
                }
            };
            var answers = {
                text: LanguageService.getValueByKey("profile_emptyState_answers_text"),
                btnText: LanguageService.getValueByKey("profile_emptyState_answers_btnText"),
                btnUrl: 'home'
            };
            return this.activeTab === 1 ? questions : answers;

        }
    },
    watch: {
        '$route': 'fetchData'
    },
    created() {
        this.fetchData();
        if(this.$router.meta && this.$router.meta.previous ){
            this.activeTab = this.$router.meta.previous
        }else{
            this.activeTab = 1
        }

    }
}