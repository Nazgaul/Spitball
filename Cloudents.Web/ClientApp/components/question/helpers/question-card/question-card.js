import userBlock from "./../../../helpers/user-block/user-block.vue";
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import reportItem from "../../../results/helpers/reportItem/reportItem.vue"
import disableForm from "../../../mixins/submitDisableMixin"
import { mapGetters, mapActions } from 'vuex'
// import timeago from 'timeago.js';
import timeAgoService from '../../../../services/language/timeAgoService';
import { LanguageService } from "../../../../services/language/languageService";

export default {
    mixins: [disableForm],
    components: {
        userBlock,
        sbDialog,
        reportItem
    },
    props: {
        hasAnswer: false,
        typeAnswer: {
            type: Boolean,
            required: false,
            default: false
        },
        showApproveButton: {
            type: Boolean,
            required: false,
            default: false
        },
        cardData: {},
        fromCarousel: {
            type: Boolean,
            required: false
        },
        suggestion: {
            type: Boolean,
            default: false
        },
        isApproved: {
            type: Boolean,
            default: false
        },
        isCorrectAnswer: {
            type: Boolean,
            default: false
        },
        detailedView: {
            type: Boolean,
            default: false
        }
    },
    data() {
        return {
            actions: [
                {
                    title: LanguageService.getValueByKey("questionCard_Report"),
                    action: this.reportItem,
                    isDisabled: this.isDisabled,
                    isVisible: true
                },
                {
                    title: LanguageService.getValueByKey("questionCard_Delete"),
                    action: this.deleteQuestion,
                    isDisabled: this.isDeleteDisabled,
                    isVisible: true
                }
            ],
            showReport: false,
            itemId: 0,
            answerToDeletObj: {},
            isDeleted: false,
            showActionToaster: false,
            localMarkedAsCorrect: false,
            toasterText: '',
            timeoutID: null,
            action: null,
            path: '',
            src: '',
        };
    },
    computed: {
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        limitedCardAnswers() {
            if (typeof  this.cardData.answers === "number") {
                if (this.cardData.answers > 3) {
                    return 3;
                } else {
                    return this.cardData.answers;
                }
            } else if (!!this.cardData && !!this.cardData.answers) {
                return this.cardData.answers.length > 3 ? this.cardData.answers.slice(0, 3) : this.cardData.answers.slice();
            }
        },
        isSold() {
            return !this.cardData.hasCorrectAnswer && !this.cardData.correctAnswerId;
        },
        cardTime() {
            return this.cardData.dateTime || this.cardData.create;
        },
        cardAnswers() {
            return this.cardData.answers;
        },
        date() {           
            return timeAgoService.timeAgoFormat(this.cardData.create);
        }
    },
    methods: {
        ...mapActions({
            'delete': 'deleteQuestion',
            correctAnswer: 'correctAnswer',
            updateToasterParams: 'updateToasterParams',
            removeQuestionItemAction: 'removeQuestionItemAction',
            manualAnswerRemove: 'answerRemoved',
        }),
        ...mapGetters(['accountUser']),

        cardOwner() {
            let user = this.accountUser();
            if (user && this.cardData.user) {
                return user.id === this.cardData.user.id; // will work once API call will also return userId
            }
            return false;
        },
        isDeleteDisabled(){
            return !this.cardOwner();
        },
        isDisabled() {
            let isOwner, account;
            isOwner = this.cardOwner();
            account = this.accountUser();
            if (isOwner || !account) {
                return true;
            }
        },
        reportItem() {
            this.itemId = this.cardData.id;
            let answerToHide = {
                questionId: parseInt(this.$route.params.id),
                answer: {
                    id: this.itemId
                }
            };
            //assign to obj passed as prop to report component
            this.answerToDeletObj =  Object.assign(answerToHide);
            this.showReport = !this.showReport;
        },
        closeReportDialog() {
            this.showReport = false;
        },
        markAsCorrect() {
            this.localMarkedAsCorrect = true;
            this.correctAnswer(this.cardData.id);
        },
        deleteQuestion() {
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')})
                .then(() => {
                    this.updateToasterParams({
                        toasterText: this.typeAnswer ? LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_answer") : LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_question"),
                        showToaster: true
                    });
                    if (!this.typeAnswer) {
                        let objToDelete = {
                            id: parseInt(this.$route.params.id)
                        };
                        this.$ga.event("Delete_question", "Homework help");
                        //ToDO change to router link use and not text URL
                        this.removeQuestionItemAction(objToDelete);
                        this.$router.push('/ask');
                    } else {
                        //emit to root to update array of answers
                        this.$ga.event("Delete_answer", "Homework help");

                        //delete object Manually
                        let answerToRemove = {
                            questionId: parseInt(this.$route.params.id),
                            answer: {
                                id: this.cardData.id
                            }
                        };
                        this.manualAnswerRemove(answerToRemove);
                        this.isDeleted = true;
                    }
                });
        }
    }
}