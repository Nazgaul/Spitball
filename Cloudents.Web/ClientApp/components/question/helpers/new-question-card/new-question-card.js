import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import userRank from "../../../helpers/UserRank/UserRank.vue";
import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
import reportItem from "../../../results/helpers/reportItem/reportItem.vue"
import { LanguageService } from "../../../../services/language/languageService";
import { mapGetters, mapActions } from 'vuex'

export default {
    components: {
        sbDialog,
        reportItem,
        userAvatar,
        userRank
    },
    data() {
        return {
            actions: [
                {
                    title: LanguageService.getValueByKey("questionCard_Report"),
                    action: this.reportItem,
                    isDisabled: this.isDisabled,
                    isVisible: true,
                    icon: 'sbf-flag'
                },
                {
                    title: LanguageService.getValueByKey("questionCard_Delete"),
                    action: this.deleteQuestion,
                    isDisabled: this.canDelete,
                    isVisible: true,
                    icon: 'sbf-delete'
                }
            ],
            showReportReasons: false,
            itemId: 0,
            maximumAnswersToDisplay: 3,
            isRtl: global.isRtl,
            showDialog: false,
            selectedImage: '',
        };
    },
    props: {
        cardData: {
            required: true
        },
        detailedView: {
            type: Boolean,
            default: false
        },
        suggestion:{
            type: Boolean,
            default: false
        }
    },
    computed: {
        userImageUrl(){
            if( this.cardData && this.cardData.user &&  this.cardData.user.image && this.cardData.user.image.length > 1){
                return `${this.cardData.user.image}`
            }
            return ''
        },
        lineClampValue(){
           if(this.detailedView && !this.suggestion){
               return 0
           }else if(this.suggestion){
               return 5
           }else{
               return 8
           }
        },
        uploadDate() {
            if (this.cardData && this.cardData.dateTime) {
                return this.$options.filters.fullMonthDate(this.cardData.dateTime);
            } else {
                return "";
            }
        },
        hideAnswerInput() {
            return this.detailedView
        },
        cursorDefault(){
            return this.detailedView && !this.suggestion
        },
        isSold() {
            return this.cardData.hasCorrectAnswer || this.cardData.correctAnswerId;
        },
        randomRank() {
            return Math.floor(Math.random() * 3);
        },
        answersNumber() {
            let answersNum = this.cardData.answers;
            let numericValue = 0;
            if (typeof answersNum !== 'number') {
                numericValue = answersNum.length
            } else {
                numericValue = answersNum;
            }
            if (numericValue > this.maximumAnswersToDisplay) {
                return this.maximumAnswersToDisplay;
            }
            return numericValue;
        },
        answersDeltaNumber() {
            let answersNum = this.cardData.answers;
            let numericValue = 0;
            if (typeof answersNum !== 'number') {
                numericValue = answersNum.length
            } else {
                numericValue = answersNum;
            }
            let delta = 0;
            if (numericValue > this.maximumAnswersToDisplay) {
                delta = numericValue - this.maximumAnswersToDisplay;
            }
            return delta;
        },
        randomViews() {
            return Math.floor(Math.random() * 1001);
        },


    },
    methods: {
        ...mapActions({
            'delete': 'deleteQuestion',
            correctAnswer: 'correctAnswer',
            updateBalance: 'updateUserBalance',
            updateToasterParams: 'updateToasterParams',
            removeQuestionItemAction: 'removeQuestionItemAction',
            manualAnswerRemove: 'answerRemoved',
            questionVote: "HomeworkHelp_questionVote",
            updateLoginDialogState: "updateLoginDialogState",
            removeItemFromProfile: "removeItemFromProfile"

        }),
        ...mapGetters(['accountUser']),
        isAuthUser() {
            let user = this.accountUser();
            if (user == null) {
                this.updateLoginDialogState(true);
                return false;
            }
            return true;
        },
        upvoteQuestion() {
            if (this.isAuthUser()) {
                let type = "up";
                if (!!this.cardData.upvoted) {
                    type = "none";
                }
                let data = {
                    type,
                    id: this.cardData.id
                };
                this.questionVote(data);
            }
        },
        downvoteQuestion() {
            if (this.isAuthUser()) {
                let type = "down";
                if (!!this.cardData.downvoted) {
                    type = "none";
                }
                let data = {
                    type,
                    id: this.cardData.id
                };
                this.questionVote(data);
            }
        },
        isDisabled() {
            let isOwner, account, notEnough;
            isOwner = this.cardOwner();
            account = this.accountUser();
            if (isOwner || !account || notEnough) {
                return true
            }
        },
        cardOwner() {
            let userAccount = this.accountUser();
            if (userAccount && this.cardData.user) {
                return userAccount.id === this.cardData.user.id; // will work once API call will also return userId
            }
        },
        canDelete() {
            let isOwner = this.cardOwner();
            if (!isOwner) {
                return true;
            }
            if (typeof this.cardData.answers !== 'number') {
                return this.cardData.answers.length !== 0;
            }
            return this.cardData.answers !== 0;


        },
        showBigImage(src) {
            this.showDialog = true;
            this.selectedImage = src;
        },
        updateProfile(objToDelete){
            if(this.$route.name === "profile"){
                this.removeItemFromProfile(objToDelete);
            }
        },
        deleteQuestion() {
            let questionId = this.cardData.id;
            this.delete({id: questionId, type: 'Question'})
                .then((success) => {
                        this.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_question"),
                            showToaster: true,
                        });
                        let objToDelete = {
                            id: parseInt(questionId)
                        };
                        this.$ga.event("Delete_question", "Homework help");
                        this.removeQuestionItemAction(objToDelete);
                        if (this.$route.name === 'question') {
                            //redirect only if question got deleted from the question page
                            this.$router.push('/ask')
                        }
                        //if profile refresh profile data
                        this.updateProfile(objToDelete);
                    },
                    (error) => {
                        console.error(error)
                        this.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("questionCard_error_delete"),
                            showToaster: true,
                        });
                    }
                );
        },
        reportItem() {
            this.itemId = this.cardData.id;
            this.showReportReasons = !this.showReportReasons;
        },
        closeReportDialog() {
            this.showReportReasons = false
        }
    },
};