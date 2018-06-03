import userBlock from "./../../../helpers/user-block/user-block.vue";
import questionService from "../../../../services/questionService";

export default {
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
        answerBtn: {
            type: Boolean,
            default: false
        },
        clickCard: {
            type: Boolean,
            default: false
        },
        isApproved: {
            type: Boolean,
            default: false
        },
        isCorrectAnswer: {
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
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
    },
    methods: {
        markAsCorrect() {
            this.$parent.markAsCorrect(this.cardData.id); //TODO: MEH :\  check if it can be done in a better way...
        },
        upVote(){
            questionService.upVote(this.cardData.id);
        },
        openQuestion(){
            return this.clickCard && this.$router.push({path:`/question/${this.cardData.id}`});
        }
    }
}