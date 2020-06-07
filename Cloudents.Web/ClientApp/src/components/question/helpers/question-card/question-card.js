import { mapGetters, mapActions } from 'vuex'

import disableForm from "../../../mixins/submitDisableMixin"

import userBlock from "./../../../helpers/user-block/user-block.vue";
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import reportItem from "../../../results/helpers/reportItem/reportItem.vue"
import timeAgoService from '../../../../services/language/timeAgoService';

export default {
    mixins: [disableForm],
    components: {
        userBlock,
        sbDialog,
        reportItem
    },
    props: {
        cardData: {},
        typeAnswer: {
            type: Boolean,
            required: false,
            default: false
        },
    },
    data() {
        return {
            actions: [
                {
                    title: this.$t("questionCard_Report"),
                    action: this.reportItem,
                    isDisabled: this.isDisabled,
                    isVisible: true
                },
                {
                    title: this.$t("questionCard_Delete"),
                    action: this.deleteQuestion,
                    isDisabled: this.isDeleteDisabled,
                    isVisible: true
                }
            ],
            showReport: false, // ok
            itemId: 0, // ok
            answerToDeletObj: {}, // ok
            isDeleted: false, // ok
            toasterText: '',
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
            // correctAnswer: 'correctAnswer',
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
        canDelete() {
           let isOwner = this.cardOwner();
           return isOwner;
        },
        deleteQuestion() {
            let self = this
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')})
                .then(() => {
                        let text = self.typeAnswer ? 'helpers_questionCard_toasterDeleted_answer' : 'helpers_questionCard_toasterDeleted_question'
                        self.updateToasterParams({
                            toasterText: self.$t(text),
                            showToaster: true
                        });
                        if (!self.typeAnswer) {
                            let objToDelete = {
                                id: parseInt(self.$route.params.id)
                            };
                            self.$ga.event("Delete_question", "Homework help");
                            //ToDO change to router link use and not text URL
                            self.removeQuestionItemAction(objToDelete);
                            self.$router.push('/ask');
                        } else {
                            //emit to root to update array of answers
                            self.$ga.event("Delete_answer", "Homework help");

                            //delete object Manually
                            let answerToRemove = {
                                questionId: parseInt(self.$route.params.id),
                                answer: {
                                    id: self.cardData.id
                                }
                            };
                            self.manualAnswerRemove(answerToRemove);
                            self.isDeleted = true;
                        }
                    }
                );
        },
    }
}