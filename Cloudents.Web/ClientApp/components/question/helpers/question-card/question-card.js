import userBlock from "./../../../helpers/user-block/user-block.vue";
import disableForm from "../../../mixins/submitDisableMixin"
import { mapGetters, mapActions } from 'vuex'
import colorsSet from '../colorsSet';
import timeago from 'timeago.js';
import { LanguageService } from "../../../../services/language/languageService";

export default {
    mixins: [disableForm],
    components: {userBlock},
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
            default: false,
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
            //limitedCardAnswers: [],
            colorsSet: colorsSet,
        }
    },
    computed: {
        ...mapGetters(['accountUser', 'getToasterText', 'getShowToaster']),
        gallery() {
            return this.cardData.files
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
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
            return this.typeAnswer ? !this.flaggedAsCorrect : !this.cardData.answers.length;
        },
        cssRuleFontColor() {
            return this.getQuestionColor("textColor")
        },
        cssRuleBackgroundColor() {
            return this.getQuestionColor("cssRule")
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
            return this.isCorrectAnswer || this.localMarkedAsCorrect
        },
        isSold() {
            return !this.cardData.hasCorrectAnswer && !this.cardData.correctAnswerId
        },
        cardTime() {
            return this.cardData.dateTime || this.cardData.create
        },
        cardAnswers() {
            return this.cardData.answers
        }
    },
    methods: {
        ...mapActions({
            'delete': 'deleteQuestion',
            correctAnswer: 'correctAnswer',
            updateBalance: 'updateUserBalance',
            updateToasterParams: 'updateToasterParams'
        }),
        getQuestionColor(type) {

            if (!!this.cardData && this.cardData.color && this.colorsSet[`${this.cardData.color}`]) {
                return this.colorsSet[`${this.cardData.color}`][type]
            } else {
                let colDefault = 'default';
                return this.colorsSet[`${colDefault}`][type]
            }
        },
        showBigImage(src) {
            this.showDialog = true;
            this.selectedImage = src;
        },
        markAsCorrect() {
            this.localMarkedAsCorrect = true;
            this.correctAnswer(this.cardData.id);
        },
        deleteQuestion() {
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')})
                .then((success) => {
                        this.updateToasterParams({
                            toasterText: this.typeAnswer ? LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_answer") : LanguageService.getValueByKey("helpers_questionCard_toasterDeleted_question"),
                            showToaster: true,
                        });
                        if (!this.typeAnswer) {
                            this.updateBalance(this.cardData.price);
                            this.$ga.event("Delete_question", "Homework help");
                            //ToDO change to router link use and not text URL
                            this.$router.push('/ask')
                        } else {
                            //emit to root to update array of answers
                            this.$ga.event("Delete_answer", "Homework help");
                            this.$root.$emit('deleteAnswer', this.cardData.id);
                            this.isDeleted = true
                        }
                    },
                    (error) => {
                        console.error(error)
                    }
                );
        },
        renderQuestionTime(className) {
            timeago().render(document.querySelectorAll(className));
        }
    },
    // created(){
    //   console.log(this.cardData)
    // },
    mounted() {
        this.renderQuestionTime('.timeago')
        // use render method to render nodes in real time
    },
    updated() {
        // when signalR adds a question we want the time to be rerendered to show correct time
        // thats why we have same function on mounted and updated
        this.renderQuestionTime('.timeago')
    },
}