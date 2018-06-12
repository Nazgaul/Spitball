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
            showDeleteDialog: false,
            flaggedAsCorrect: false

            ,gallery:['https://www.howtogeek.com/wp-content/uploads/2009/08/image27.png','https://wallpaper-house.com/data/out/12/wallpaper2you_501354.jpg','https://wallpaper-house.com/data/out/8/wallpaper2you_257166.jpg']
        }
    },

    computed: {
        ...mapGetters(['accountUser']),
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
            this.delete(this.cardData.id).then((val) => {
                val ? this.$router.push('/ask') : this.showDelete = true;
            })
        },
        markAsCorrect() {
            this.flaggedAsCorrect = true;
            this.$parent.markAsCorrect(this.cardData.id); //TODO: MEH :\  check if it can be done in a better way...
        }
    },
    created() {
        this.flaggedAsCorrect = this.isCorrectAnswer;
        if (!this.cardData.user){
            this.cardData.user = {id:539,name:"JUST FOR TESTING"}
        }

    }
}