import userBlock from "./../../../helpers/user-block/user-block.vue";
import disableForm from "../../../mixins/submitDisableMixin"
import {mapGetters, mapActions} from 'vuex'

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
            user: {
                img: 'https://cdn.pixabay.com/photo/2016/08/20/05/38/avatar-1606916_960_720.png',
                name: 'User Name'
            },
            showDelete: false,
            dialogDeleteUserText: "you should select right answer"
        }
    },

    computed: {
        ...mapGetters(['accountUser']),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        myQuestion() {
            if (this.accountUser && this.cardData.user) {
                return this.accountUser.id === this.cardData.user.id; // will work once API call will also return userId
            }
        },
        haveAnswers() {
            return this.cardData.answers.length
        }
    },
    methods: {
        ...mapActions({'delete': 'deleteQuestion'}),
        deleteQuestion() {
            this.dialogDeleteUserText = "there was issue in the delete";
            this.delete(this.cardData.id).then((val) => {
                val ? this.$router.push('/ask') : this.showDelete = true;
            })
        },
        markAsCorrect() {
            this.$parent.markAsCorrect(this.cardData.id); //TODO: MEH :\  check if it can be done in a better way...
        }
    }
}