

//old
import questionCard from "../question/helpers/new-question-card/new-question-card.vue";
import resultNote from "../results/ResultNote.vue"
import userBlock from '../helpers/user-block/user-block.vue';
import { mapActions, mapGetters } from 'vuex'
import { LanguageService } from "../../services/language/languageService";
import uploadDocumentBtn from "../results/helpers/uploadFilesBtn/uploadFilesBtn.vue"
//old
//new
import profileBio from './profileHelpers/profileBio/profileBio.vue';
import tutorAboutMe from './profileHelpers/profileAbout/tutorAboutMe.vue';
import coursesCard from './profileHelpers/coursesCard/coursesCard.vue';
import subjectsCard from './profileHelpers/subjectsCard/subjectsCard.vue';
import reviewsList from './profileHelpers/reviews/reviewsList.vue';
import tutorInfoBlock from './profileHelpers/tutoringInfo/tutorInfoBlock.vue'


//new
export default {
    name: "new_profile",
    components: {
        questionCard,
        userBlock,
        resultNote,
        uploadDocumentBtn,
        profileBio,
        tutorAboutMe,
        coursesCard,
        subjectsCard,
        reviewsList,
        tutorInfoBlock
    },
    props: {
        id: {
            Number
        }
    },
    data() {
        return {
            isRtl: global.isRtl,
            isEdgeRtl: global.isEdgeRtl,
            loadingContent: false,
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
            },
            purchasedDocuments: {
                isLoading: false,
                isComplete: false,
                page: 1
            }
        }
    },
    methods: {
        ...mapActions([
            'updateNewQuestionDialogState',
            'syncProfile',
            'getAnswers',
            'getQuestions',
            'getDocuments',
            'resetProfileData',
            'getPurchasedDocuments',
            'setProfileByActiveTab'
        ]),

        changeActiveTab(tabId) {
            this.activeTab = tabId;
        },
        fetchData() {
            let syncObj = {
                id: this.id,
                activeTab: this.activeTab
            }
            this.syncProfile(syncObj);
        },
        getInfoByTab() {
            this.loadingContent = true;
            this.setProfileByActiveTab(this.activeTab).then(() => {
                this.loadingContent = false;
            })
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
        },
        loadPurchasedDocuments() {
            if (this.profileData.purchasedDocuments.length < this.itemsPerTab) {
                this.purchasedDocuments.isComplete = true;
                return;
            }
            this.purchasedDocuments.isLoading = true;
            let DocumentsInfo = {
                id: this.id,
                page: this.purchasedDocuments.page,
                user: this.profileData.user
            }
            this.getPurchasedDocuments(DocumentsInfo).then((hasData) => {
                if (!hasData) {
                    this.purchasedDocuments.isComplete = true;
                }
                this.purchasedDocuments.isLoading = false;
                this.purchasedDocuments.page++;
            }, (err) => {
                this.purchasedDocuments.isComplete = true;
            })
        }
    },
    computed: {
        ...mapGetters(["accountUser", "getProfile"]),
        xsColumn(){
            const xsColumn = {};
            if (this.$vuetify.breakpoint.xsOnly){
                xsColumn.column = true;
            }
            return xsColumn
        },
        profileData() {
            if (!!this.getProfile) {
                return this.getProfile
            }
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        isMyProfile() {
            if (!!this.profileData) {
                return false  // return this.accountUser && this.accountUser.id && this.profileData ? this.profileData.user.id == this.accountUser.id : false;
            }
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
                btnUrl: 'ask'
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
        '$route': 'fetchData',
        activeTab() {
            this.getInfoByTab();
        }
    },
    //reset profile data to prevent glitch in profile loading
    beforeRouteLeave(to, from, next) {
        this.resetProfileData();
        next()
    },
    created() {
        this.fetchData();
    }
}

