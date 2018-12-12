import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import userRank from "../../../helpers/UserRank/UserRank.vue";
import { LanguageService } from "../../../../services/language/languageService";
import { mapGetters, mapActions } from 'vuex'
export default {
    data() {
        return {
            actions: [
                {
                    title: LanguageService.getValueByKey("questionCard_Report"),
                    action: this.reportItem,
                    isVisible: !this.cardOwner()},
                {
                    title: LanguageService.getValueByKey("questionCard_Delete"),
                    action: this.deleteQuestion,
                    isVisible: this.canDelete()
                }
            ],
            maximumAnswersToDisplay: 3,
            isRtl: global.isRtl
        };
    },
    props: {
        cardData: {
            required: true
        }
    },
    components: {
        userAvatar,
        userRank
    },
    computed: {
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
            manualAnswerRemove: 'answerRemoved',
            questionVote: "questionVote"
        }),
        ...mapGetters(['accountUser']),
        upvoteQuestion(){
            let type = "up";
            if(!!this.cardData.upvoted){
                type = "none"; 
            }
            let data = {
                type,
                id: this.cardData.id
            }
            this.questionVote(data);
        },
        downvoteQuestion(){
            let type = "down";
            if(!!this.cardData.downvoted){
                type = "none"; 
            }
            let data = {
                type,
                id: this.cardData.id
            }
            this.questionVote(data);
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
                return false;
            }
            return  this.cardData.answers < 1 && !this.cardData.answers.length;
        },
        deleteQuestion() {
            this.delete({id: this.cardData.id, type:  'Question'})
                .then((success) => {
                        this.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_question"),
                            showToaster: true,
                        });
                        let objToDelete = {
                            id: parseInt(this.$route.params.id)
                        }
                        this.updateBalance(this.cardData.price);
                        this.$ga.event("Delete_question", "Homework help");
                        //ToDO change to router link use and not text URL
                        this.removeQuestionItemAction(objToDelete);
                        if(this.$route.name === 'question'){
                            //redirect only if question got deleted from the question page
                            this.$router.push('/ask')
                        }
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
        reportItem(){
            console.log('reporting item', {id: this.cardData.id, type:  'Question'});
            this.updateToasterParams({
                toasterText: LanguageService.getValueByKey("questionCard_Report"),
                showToaster: true,
            });
        }
    },
};