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
            toasterText: '',
            timeoutID: null,
            action: null,
            path: ''
        }
    },
    watch: {
        isUndoAction(val) {
            if (val) {
                clearTimeout(this.timeoutID);
                this.updateParams({showToaster: false});
            }
        }
    },
    computed: {
        ...mapGetters(['accountUser', 'isUndoAction', 'getToasterText', 'getShowToaster', 'getToasterTimeOut']),
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
        markAsCorrect() {
            // this.performAction(this.markAsCorrectAction, 'You marked this answer has a correct answer ');
            var toasterText = this.typeAnswer ? 'The answer has been deleted' : 'The question has been deleted';
            this.updateParams({
                toasterText: toasterText,
                showToaster: true,
                undoReturnPath: this.$route.path,
                undoAction: function () {
                    console.log('here comes an API call to revert the delete - case 10292 assigned to Ram');
                }
            });
            this.flaggedAsCorrect = true;
            this.correctAnswer(this.cardData.id);
            this.updateParams({toasterText: '', showToaster: false});
        },
        deleteQuestion() {
            this.updateParams({
                toasterText: this.typeAnswer ? 'The answer has been deleted' : 'The question has been deleted',
                showToaster: true,
                undoReturnPath: this.$route.path,
                undoAction: function () {
                    console.log('here comes an API call to revert the delete - case 10292 assigned to Ram');
                }
            });
            var self = this;
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')}).then(() => {
                debugger;
                !this.typeAnswer ? this.$router.push('/ask') : this.isDeleted = true
            });
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