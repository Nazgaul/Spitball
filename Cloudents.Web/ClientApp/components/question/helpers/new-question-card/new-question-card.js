import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
import reportItem from "../../../results/helpers/reportItem/reportItem.vue"
import { LanguageService } from "../../../../services/language/languageService";
import { mapGetters, mapActions } from 'vuex';
import questionNote from './question-note.svg';

export default {
    components: {
        sbDialog,
        reportItem,
        questionNote,
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
                    action: this.removeQuestion,
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
            isQuestionPage: false,
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
        ...mapGetters(['accountUser']),

        userImageUrl(){
            if( this.cardData && this.cardData.user &&  this.cardData.user.image && this.cardData.user.image.length > 1){
                return `${this.cardData.user.image}`;
            }
            return '';
        },
        uploadDate() {
            if (this.cardData && this.cardData.dateTime) {
                return this.$options.filters.fullMonthDate(this.cardData.dateTime);
            }
            return "";
        },
        uploadDateAnswer() {
            if (this.cardData.firstAnswer && this.cardData.firstAnswer.date) {
                
                return this.$options.filters.fullMonthDate(this.cardData.firstAnswer.date);
            }
            return "";
        },
        hideAnswerInput() {           
            return this.detailedView;
        },
        isSold() {            
            return this.cardData.hasCorrectAnswer || this.cardData.correctAnswerId;
        },
        answersCount() {
            return this.cardData.answers;
        },
        answers() {           
            return this.cardData.firstAnswer;
        },
        answersNumber() {
            let answersNum = this.cardData.answers;
            let numericValue;
            if (typeof answersNum !== 'number') {
                numericValue = answersNum.length;
            } else {
                numericValue = answersNum;
            }
            if (numericValue > this.maximumAnswersToDisplay) {
                return this.maximumAnswersToDisplay;
            }
            return numericValue;
        },
        answersDeltaNumber() {
            let answersNum = this.cardData.answers || 1;
            let numericValue;
            if (typeof answersNum !== 'number') {
                numericValue = answersNum.length;
            } else {
                numericValue = answersNum;
            }
            let delta = 0;
            if (numericValue > this.maximumAnswersToDisplay) {
                delta = numericValue - this.maximumAnswersToDisplay;
            }
            return delta;
        },
        moreAnswersDictionary() {
            return this.cardData.answers > 1 ? 'questionCard_Answers' : 'questionCard_Answer_one';
        }
        
    },
    methods: {
        ...mapActions([
            'deleteQuestion',
            // 'correctAnswer',
            'updateToasterParams',
            'removeQuestionItemAction',
            'removeItemFromProfile'
        ]),

        isDisabled() {
            let isOwner = this.cardOwner();
            let account = this.accountUser;
            if (isOwner || !account ) {
                return true;
            }
            return false;

        },
        cardOwner() {
            let userAccount = this.accountUser;
            if (userAccount && this.cardData.userId) {
                return userAccount.id === this.cardData.userId; // will work once API call will also return userId
            }
            return false;
        },
        canDelete() {
            let isOwner = this.cardOwner();
            if (!isOwner) {
                return true;
            } else{
                return false;
            }
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
        removeQuestion() {
            let questionId = this.cardData.id;
            this.deleteQuestion({id: questionId, type: 'Question'}).then(() => {
                this.updateToasterParams({
                    toasterText: LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_question"),
                    showToaster: true
                });
                let objToDelete = {
                    id: parseInt(questionId)
                };
                this.$ga.event("Delete_question", "Homework help");
                this.removeQuestionItemAction(objToDelete);
                if (this.$route.name === 'question') {
                    //redirect only if question got deleted from the question page
                    this.$router.push('/');
                }
                //if profile refresh profile data
                this.updateProfile(objToDelete);
            },
            (error) => {
                console.error(error);
                this.updateToasterParams({
                    toasterText: LanguageService.getValueByKey("questionCard_error_delete"),
                    showToaster: true
                });
            });
        },
        reportItem() {
            this.itemId = this.cardData.id;
            this.showReportReasons = !this.showReportReasons;
        },
        closeReportDialog() {
            this.showReportReasons = false;
        }
    },
    created() {
        this.isQuestionPage = (this.$route.name === 'question');
    },
};