import questionCard from "../question/helpers/question-card/question-card.vue";
import resultNote from "../results/ResultNote.vue"
import userBlock from '../helpers/user-block/user-block.vue';
import { mapGetters, mapActions } from 'vuex'
import { LanguageService } from "../../services/language/languageService";
import uploadDocumentBtn from "../results/helpers/uploadFilesBtn/uploadFilesBtn.vue"
export default {
    components: {questionCard, userBlock, resultNote, uploadDocumentBtn},
    props: {
        id: {Number}
    },
    data() {
        return {
            activeTab: 1,
            itemsPerTab: 50,
            answers: {
                isLoading: false,
                isComplete: false,
                page: 1,
            },
            questions: {
                isLoading: false,
                isComplete: false,
                page: 1,
            },
            documents: {
                isLoading: false,
                isComplete: false,
                page: 1
            }
        }
    },
    methods: {
        ...mapActions(['updateNewQuestionDialogState', 'syncProfile', 'getAnswers', 'getQuestions', 'getDocuments']),

        changeActiveTab(tabId) {
            this.activeTab = tabId;
            this.$router.meta = {previous: tabId}
        },
        fetchData() {
            this.syncProfile(this.id);
        },
        loadAnswers() {
            if (this.profileData.answers.length < this.itemsPerTab) {
                this.answers.isComplete = true;
                return;
            }
            this.answers.isLoading = true;
            let AnswersInfo = {
                id: this.id,
                page: this.answers.page
            }
            this.getAnswers(AnswersInfo).then((hasData) => {
                if (!hasData) {
                    this.answers.isComplete = true;
                }
                this.answers.isLoading = false;
                this.answers.page++;
            }, (err) => {
                this.answers.isComplete = true;
            })
        },
        loadQuestions() {
            if (this.profileData.questions.length < this.itemsPerTab) {
                this.questions.isComplete = true;
                return;
            }
            this.questions.isLoading = true;
            let QuestionsInfo = {
                id: this.id,
                page: this.questions.page,
                user: this.profileData.user
            }
            this.getQuestions(QuestionsInfo).then((hasData) => {
                if (!hasData) {
                    this.questions.isComplete = true;
                }
                this.questions.isLoading = false;
                this.questions.page++;
            }, (err) => {
                this.questions.isComplete = true;
            })
        },
        loadDocuments() {
            if (this.profileData.documents.length < this.itemsPerTab) {
                this.documents.isComplete = true;
                return;
            }
            this.documents.isLoading = true;
            let DocumentsInfo = {
                id: this.id,
                page: this.documents.page,
                user: this.profileData.user
            }
            this.getDocuments(DocumentsInfo).then((hasData) => {
                if (!hasData) {
                    this.documents.isComplete = true;
                }
                this.documents.isLoading = false;
                this.documents.page++;
            }, (err) => {
                this.documents.isComplete = true;
            })
        }
    },
    computed: {
        ...mapGetters(["accountUser", "getProfile"]),
        profileData() {
            return this.getProfile;
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        isMyProfile() {
            return this.accountUser && this.accountUser.id && this.profileData ? this.profileData.user.id == this.accountUser.id : false;
        },
        emptyStateData() {
            let questions = {
                text: LanguageService.getValueByKey("profile_emptyState_questions_text"),
                boldText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                btnText: LanguageService.getValueByKey("profile_emptyState_questions_btnText"),
                btnUrl: () => {
                    let Obj = {
                        status: true,
                        from: 5
                    };
                    this.updateNewQuestionDialogState(Obj)
                }
            };
            let answers = {
                text: LanguageService.getValueByKey("profile_emptyState_answers_text"),
                btnText: LanguageService.getValueByKey("profile_emptyState_answers_btnText"),
                btnUrl: 'home'
            };
            let documents = {
                text: LanguageService.getValueByKey("profile_emptyState_documents_text"),
                //TODO feel free to remove this after redesign, will not be used, using reusable component instead
                btnText: LanguageService.getValueByKey("profile_emptyState_documents_btnText"),
                btnUrl: 'note'
            };
            if (this.activeTab === 1) {
                return questions
            } else if (this.activeTab === 2) {
                return answers
            } else if (this.activeTab === 3) {
                return documents
            }
        }
    },
    watch: {
        '$route': 'fetchData'
    },
    created() {
        this.fetchData();
        if (this.$router.meta && this.$router.meta.previous) {
            this.activeTab = this.$router.meta.previous
        } else {
            this.activeTab = 1
        }
    }
}