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
            showDeleteDialog: false,
            flaggedAsCorrect: false
        }
    },

    computed: {
        ...mapGetters(['accountUser']),
        gallery(){return this.cardData.files},
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        cardOwner() {
            if (this.accountUser && this.cardData.user) {
                return this.accountUser.id === this.cardData.user.id; // will work once API call will also return userId
            }
        },
        haveAnswers() {
            return this.cardData.answers.length
        },
        canDelete(){
            if(!this.cardOwner){
                return false;
            }
            return this.typeAnswer ? !this.flaggedAsCorrect : !this.cardData.answers.length;
        }
    },
    methods: {
        ...mapActions({'delete': 'deleteQuestion'}),
        deleteQuestion() {
            this.delete({id:this.cardData.id,type:(this.typeAnswer?'Answer':'Question')}).then(() => {
                 this.$router.push('/ask')
            })
        },
        markAsCorrect() {
            this.flaggedAsCorrect = true;
            this.$parent.markAsCorrect(this.cardData.id); //TODO: MEH :\  check if it can be done in a better way...
        }
    },
    mounted(){

        timeago().render(document.querySelectorAll('.timeago'));
// use render method to render nodes in real time
    },
    created() {
        this.flaggedAsCorrect = this.isCorrectAnswer;
        //TODO: what is that
        if (!this.cardData.user){
            this.cardData.user = {id:539,name:"JUST FOR TESTING"}
        }

    }
}