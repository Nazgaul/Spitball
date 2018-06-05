import userBlock from "./../../../helpers/user-block/user-block.vue";
import questionService from "../../../../services/questionService";
import disableForm from "../../../mixins/submitDisableMixin"
import {mapGetters} from "vuex"

export default {
    mixins:[disableForm],
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
        }
    },

    computed: {
        ...mapGetters(['accountUser']),
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        myQuestion(){
            return this.accountUser || this.accountUser.id === this.cardData.userId; // will work once API call will also return userId
        }
    },
    methods: {
        markAsCorrect() {
            this.$parent.markAsCorrect(this.cardData.id); //TODO: MEH :\  check if it can be done in a better way...
        },
        upVote(){
           this.submitForm()?questionService.upVote(this.cardData.id):'';
        },
    }
}