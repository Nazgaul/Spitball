import userBlock from "./../../../helpers/user-block/user-block.vue";
import disableForm from "../../../mixins/submitDisableMixin"
import {mapGetters, mapActions} from 'vuex'
import timeago from 'timeago.js';


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
            flaggedAsCorrect: false,
            toasterText: '',
            timeoutID: null,
            action: null,
            path: '',
            src: '',
            selectedImage: '',
            showDialog: false,
            limitedCardAnswers: []
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
    },
    methods: {
        ...mapActions({
            'delete': 'deleteQuestion',
            correctAnswer: 'correctAnswer',
            updateBalance: 'updateUserBalance',
            updateToasterParams: 'updateToasterParams'
        }),
        showBigImage(src) {
            this.showDialog = true;
            this.selectedImage = src;
        },
        markAsCorrect() {
            var toasterText = this.typeAnswer ? 'The answer has been deleted' : 'The question has been deleted';
            this.updateToasterParams({
                toasterText: toasterText,
                showToaster: true,
            });
            this.flaggedAsCorrect = true;
            this.correctAnswer(this.cardData.id);
            this.updateToasterParams({toasterText: '', showToaster: false});//test123

        },
        deleteQuestion() {
            this.updateToasterParams({
                toasterText: this.typeAnswer ? 'The answer has been deleted' : 'The question has been deleted',
                showToaster: true,
            });
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')})
                .then(() => {
                    if (!this.typeAnswer) {
                        this.updateBalance(this.cardData.price);
                        //To DO change to router link use and not text URL
                        this.$router.push('/ask')
                    } else {
                        //emit to root to update array of answers
                        this.$root.$emit('deleteAnswer', this.cardData.id);
                        this.isDeleted = true
                    }
                });
        },
        calculateAnswerToLimit() {
            // limit card answer could be a number or array depends on route(view)
            if (typeof  this.cardData.answers === "number") {
                if (this.cardData.answers > 3) {
                    this.limitedCardAnswers = 3;
                } else {
                    this.limitedCardAnswers = this.cardData.answers;
                }
            } else if (!!this.cardData && !!this.cardData.answers) {
                this.limitedCardAnswers = this.cardData.answers.length > 3 ? this.cardData.answers.slice(0, 3) : this.cardData.answers.slice();
            }
        }
    },
    mounted() {
        timeago().render(document.querySelectorAll('.timeago'));
// use render method to render nodes in real time
    },
    created() {
        this.flaggedAsCorrect = this.isCorrectAnswer;
        this.calculateAnswerToLimit();
    }
}