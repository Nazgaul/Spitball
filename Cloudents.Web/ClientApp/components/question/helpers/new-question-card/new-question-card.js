import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import userRank from "../../../helpers/UserRank/UserRank.vue";
import { LanguageService } from "../../../../services/language/languageService";
import { mapGetters, mapActions } from 'vuex'
export default {
    data() {
        return {
            actions: [
                {title: "Report this item"},
                {
                    title: LanguageService.getValueByKey("questionCard_Delete"),
                    action: this.delete()
                }
            ],
            maximumAnswersToDisplay: 3,
            isRtl: global.isRtl
        };
    },
    props: {
        cardData: {
            required: true
        },
        typeAnswer: {
            type: Boolean,
            required: false,
            default: false
        },
    },
    components: {
        userAvatar,
        userRank
    },
    computed: {
        ...mapGetters(['accountUser']),
        uploadDate() {
            if (this.cardData && this.cardData.dateTime) {
                return this.$options.filters.fullMonthDate(this.cardData.dateTime);
            } else {
                return "";
            }
        },
        hideAnswerInput(){
            return this.detailedView
        },
        isSold() {
            return this.cardData.hasCorrectAnswer;
        },
        cardOwner() {
            if (this.accountUser && this.cardData.user) {
                return this.accountUser.id === this.cardData.user.id; // will work once API call will also return userId
            }
        },
        canDelete() {
            if (!this.cardOwner) {
                return false;
            }
            return  !this.isSold && !this.cardData.answers.length;
        },
        cardPrice() {
            if (!!this.cardData && !!this.cardData.price) {
                return this.cardData.price.toFixed(2);
            }
        },
        randomRank() {
            return Math.floor(Math.random() * 3);
        },
        answersNumber() {
            let answersNum = this.cardData.answers;
            if (answersNum > this.maximumAnswersToDisplay) {
                return this.maximumAnswersToDisplay;
            }
            return answersNum;
        },
        answersDeltaNumber() {
            let answersNum = this.cardData.answers;
            let delta = 0;
            if (answersNum > this.maximumAnswersToDisplay) {
                delta = answersNum - this.maximumAnswersToDisplay;
            }
            return delta;
        },
        randomViews() {
            return Math.floor(Math.random() * 1001);
        },
        questionReputation() {
            return Math.floor(Math.random() * 100);
        }
    },
    methods: {
        ...mapActions({
            'delete': 'deleteQuestion',
            correctAnswer: 'correctAnswer',
            updateBalance: 'updateUserBalance',
            updateToasterParams: 'updateToasterParams',
            removeQuestionItemAction: 'removeQuestionItemAction',
            manualAnswerRemove: 'answerRemoved'
        }),
        deleteQuestion() {
            this.delete({id: this.cardData.id, type:  'Question'})
                .then((success) => {
                        this.updateToasterParams({
                            toasterText: this.typeAnswer ? LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_answer") : LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_question"),
                            showToaster: true,
                        });
                        if (!this.typeAnswer) {
                            let objToDelete = {
                                id: parseInt(this.$route.params.id)
                            }
                            this.updateBalance(this.cardData.price);
                            this.$ga.event("Delete_question", "Homework help");
                            //ToDO change to router link use and not text URL
                            this.removeQuestionItemAction(objToDelete)
                            this.$router.push('/ask')
                        } else {
                            //emit to root to update array of answers
                            this.$ga.event("Delete_answer", "Homework help");

                            //delete object Manually
                            let answerToRemove = {
                                questionId: parseInt(this.$route.params.id),
                                answer: {
                                    id: this.cardData.id}
                            }
                            this.manualAnswerRemove(answerToRemove);
                            this.isDeleted = true
                        }
                    },
                    (error) => {
                        console.error(error)
                    }
                );
        },
    },
};