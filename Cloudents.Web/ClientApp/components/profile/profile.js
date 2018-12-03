import questionCard from "../question/helpers/question-card/question-card.vue";
import resultNote from "../results/ResultNote.vue"
import userBlock from '../helpers/user-block/user-block.vue';
import {dollarCalculate} from "../../store/constants";
import accountService from '../../services/accountService';
import {mapGetters, mapActions} from 'vuex'
import { LanguageService } from "../../services/language/languageService";

export default {
    components: {questionCard, userBlock, resultNote},
    props: {
        id: {Number}
    },
    data() {
        return {
            activeTab: 1,
            itemsPerTab: 50,
            answers:{
                isLoading: false,
                isComplete: false,
                page: 1,
            },
            questions:{
                isLoading: false,
                isComplete: false,
                page: 1,
            },
        }
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState', 'syncProfile', 'getAnswers', 'getQuestions']),

        changeActiveTab(tabId) {
            this.activeTab = tabId;
            this.$router.meta = {previous : tabId}
        },
        fetchData() {
            this.syncProfile(this.id);
        },
        loadAnswers(){
            if(this.profileData.answers < this.itemsPerTab) {
                this.answers.isComplete = true;
                return;
            }
            this.answers.isLoading = true;
            let AnswersInfo = {
                id: this.id,
                page: this.answers.page
            }
            this.getAnswers(AnswersInfo).then((hasData)=>{
                if(!hasData) {
                    this.answers.isComplete = true;
                }
                this.answers.isLoading = false;
                this.answers.page++;
            },(err)=>{
                this.answers.isComplete = true;
            })
        },
        loadQuestions(){
            if(this.profileData.questions < this.itemsPerTab) {
                this.questions.isComplete = true;
                return;
            }
            this.questions.isLoading = true;
            let QuestionsInfo = {
                id: this.id,
                page: this.questions.page,
                user: this.profileData.user
            }
            this.getQuestions(QuestionsInfo).then((hasData)=>{
                if(!hasData) {
                    this.questions.isComplete = true;
                }
                this.questions.isLoading = false;
                this.questions.page++;
            },(err)=>{
                this.questions.isComplete = true;
            })
        }
    },
    computed: {
        ...mapGetters(["accountUser", "getProfile"]),
        profileData(){
            return this.getProfile;
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        isMyProfile() {
            return this.accountUser && this.accountUser.id && this.profileData ? this.profileData.user.id == this.accountUser.id : false;
        },
        emptyStateData() {
            var questions = {
                text: LanguageService.getValueByKey("profile_emptyState_questions_text"),
                boldText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                btnText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                btnUrl:  ()=> {
                    let Obj = {
                        status:true,
                        from: 5
                    }
                    this.updateNewQuestionDialogState(Obj)
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