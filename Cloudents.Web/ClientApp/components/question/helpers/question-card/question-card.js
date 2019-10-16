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
            selectedImage: '',
            showDialog: false,
            isRtl: global.isRtl
        };
    },
    computed: {
        gallery() {
            return this.cardData.files;
        },
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
        flaggedAsCorrect() {
            return this.isCorrectAnswer || this.localMarkedAsCorrect;
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
            updateBalance: 'updateUserBalance',
            updateToasterParams: 'updateToasterParams',
            removeQuestionItemAction: 'removeQuestionItemAction',
            manualAnswerRemove: 'answerRemoved',
            answerVote: 'answerVote',
            updateLoginDialogState: 'updateLoginDialogState'
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
            let isDeleteble = this.canDelete();
            return !isDeleteble;

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
        isAuthUser() {
            let user = this.accountUser();
            if (user == null) {
                this.updateLoginDialogState(true);
                return false;
            }
            return true;
        },
        upvoteAnswer() {
            if (this.isAuthUser()) {
                let type = "up";
                if (!!this.cardData.upvoted) {
                    type = "none";
                }
                let data = {
                    type,
                    id: this.cardData.id
                };
                this.answerVote(data);
            }
        },
        downvoteAnswer() {
            if (this.isAuthUser()) {
                let type = "down";
                if (!!this.cardData.downvoted) {
                    type = "none";
                }
                let data = {
                    type,
                    id: this.cardData.id
                };
                this.answerVote(data);
            }
        },
        getQuestionColor() {
            if (!!this.cardData && !this.cardData.color) {
                return this.cardData.color = 'default';
            }
        },
        showBigImage(src) {
            this.showDialog = true;
            this.selectedImage = src;
        },

        canDelete() {
           let isOwner = this.cardOwner();
           return !this.flaggedAsCorrect && isOwner;

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
                    }
                );
        },
        // renderQuestionTime(className) {
        //     const hebrewLang = function (number, index) {
        //         return [
        //             ['זה עתה', 'עכשיו'],
        //             ['לפני %s שניות', 'בעוד %s שניות'],
        //             ['לפני דקה', 'בעוד דקה'],
        //             ['לפני %s דקות', 'בעוד %s דקות'],
        //             ['לפני שעה', 'בעוד שעה'],
        //             ['לפני %s שעות', 'בעוד %s שעות'],
        //             ['אתמול', 'מחר'],
        //             ['לפני %s ימים', 'בעוד %s ימים'],
        //             ['לפני שבוע', 'בעוד שבוע'],
        //             ['לפני %s שבועות', 'בעוד %s שבועות'],
        //             ['לפני חודש', 'בעוד חודש'],
        //             ['לפני %s חודשים', 'בעוד %s חודשים'],
        //             ['לפני שנה', 'בעוד שנה'],
        //             ['לפני %s שנים', 'בעוד %s שנים']
        //         ][index];
        //     };
        //     timeago.register('he', hebrewLang);
        //     let timeAgoRef = timeago();
        //     let locale = global.lang.toLowerCase() === 'he' ? 'he' : '';
        //     timeAgoRef.render(document.querySelectorAll(className), locale);
        // }
    },
    created() {
        this.getQuestionColor();
    },
    mounted() {
        // use render method to render nodes in real time
        // this.renderQuestionTime('.timeago');
    },
    updated() {
        // when signalR adds a question we want the time to be rerendered to show correct time
        // thats why we have same function on mounted and updated
        // this.renderQuestionTime('.timeago');


    }
}