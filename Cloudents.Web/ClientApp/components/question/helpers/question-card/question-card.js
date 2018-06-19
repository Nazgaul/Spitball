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
        }
    },
    methods: {
        ...mapActions({
            'delete': 'deleteQuestion',
            correctAnswer: 'correctAnswer',updateBalance:'updateUserBalance',
            updateParams: 'updateParams'
        }),
        markAsCorrect() {
            var toasterText = this.typeAnswer ? 'The answer has been deleted' : 'The question has been deleted';
            this.updateParams({
                toasterText: toasterText,
                showToaster: true,
            });
            this.flaggedAsCorrect = true;
            this.correctAnswer(this.cardData.id);
            this.updateParams({toasterText: '', showToaster: false});
        },
        deleteQuestion() {
            this.updateParams({
                toasterText: this.typeAnswer ? 'The answer has been deleted' : 'The question has been deleted',
                showToaster: true,
            });
            this.delete({id: this.cardData.id, type: (this.typeAnswer ? 'Answer' : 'Question')}).then(() => {
                if(!this.typeAnswer ){
                    this.updateBalance(this.cardData.price);
                    this.$router.push('/ask')}else{ this.isDeleted = true}
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