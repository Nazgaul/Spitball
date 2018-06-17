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
    computed: {
        ...mapGetters(['accountUser']),
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
        // haveAnswers() {
        //     return this.cardData.answers.length
        // },
        canDelete() {
            if (!this.cardOwner) {
                return false;
            }
            return this.typeAnswer ? !this.flaggedAsCorrect : !this.cardData.answers.length;
        },
    },
    methods: {
        ...mapActions({'delete': 'deleteQuestion', correctAnswer: 'correctAnswer'}),
        deleteQuestion() {
            var toasterText = this.typeAnswer ? 'The answer has been deleted' : 'The question has been deleted';
            this.performAction(this.deleteAction, toasterText);
        },
        markAsCorrect() {
            this.performAction(this.markAsCorrectAction, 'You marked this answer has a correct answer ');
        },
        performAction(action, toasterText){
            this.toasterText = toasterText;
            this.showActionToaster = true;
            this.action = action;
            this.timeoutID = setTimeout(() =>{
                this.action()
            }, this.toasterTimeOut);
        },
        markAsCorrectAction() {
            this.flaggedAsCorrect = true;
            this.correctAnswer(this.cardData.id);
        },
        deleteAction() {
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')}).then(() => {
                !this.typeAnswer ? this.$router.push('/ask') : this.isDeleted = true
            })
        },
        closeToaster() {
            clearTimeout(this.timeoutID);
            this.action();
            this.showActionToaster = false;
        },
        undoAction() {
            clearTimeout(this.timeoutID);
            this.showActionToaster = false;
        },
    },
    mounted() {

        timeago().render(document.querySelectorAll('.timeago'));
// use render method to render nodes in real time
    },
    created() {
        this.flaggedAsCorrect = this.isCorrectAnswer;

    }
}