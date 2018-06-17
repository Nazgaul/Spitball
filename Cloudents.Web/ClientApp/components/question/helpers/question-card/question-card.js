import userBlock from "./../../../helpers/user-block/user-block.vue";
import disableForm from "../../../mixins/submitDisableMixin"
import {mapGetters, mapActions} from 'vuex'
import timeago from 'timeago.js';


export default {
    mixins: [disableForm],
    components: {userBlock},
    props: {
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
            toasterTimeOut: 5000,
            toasterText: '',
            timeoutID: null,
            action: null
        }
    },
    watch: {
        isUndoAction(val) {
            if (val) {
                clearTimeout(this.timeoutID);
                this.updateParams({showToaster: false});
            }
        },
        isCloseToaster(val) {
            if (val) {
                clearTimeout(this.timeoutID);
                this.action();
                this.updateParams({showToaster: false});
            }
        }
    },
    computed: {
        ...mapGetters(['accountUser', 'isUndoAction', 'isCloseToaster', 'getToasterText', 'getShowToaster','getToasterTimeOut']),
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
        }
    },
    methods: {
        ...mapActions({
            'delete': 'deleteQuestion',
            correctAnswer: 'correctAnswer',
            updateParams: 'updateParams'
        }),
        deleteQuestion() {
            var toasterText = this.typeAnswer ? 'The answer has been deleted' : 'The question has been deleted';
            this.performAction(this.deleteAction, toasterText);
        },
        markAsCorrect() {
            this.performAction(this.markAsCorrectAction, 'You marked this answer has a correct answer ');
        },
        performAction(action, toasterText) {
            this.updateParams({toasterText: toasterText, showToaster: true});
            this.action = action;
            this.timeoutID = setTimeout(() => {
                this.action()
            }, this.getToasterTimeOut);

        },
        markAsCorrectAction() {
            this.flaggedAsCorrect = true;
            this.correctAnswer(this.cardData.id);
            this.updateParams({toasterText: '', showToaster: false});
        },
        deleteAction() {
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')}).then(() => {
                !this.typeAnswer ? this.$router.push('/ask') : this.isDeleted = true
            })
            this.updateParams({toasterText: '', showToaster: false});

        },
    },
    mounted() {

        timeago().render(document.querySelectorAll('.timeago'));
// use render method to render nodes in real time
    },
    created() {
        this.flaggedAsCorrect = this.isCorrectAnswer;
        this.updateParams({toasterTimeOut: 5000000});

    }
}