import questionCard from "../question/helpers/question-card/question-card.vue";
import {mapActions} from 'vuex'
import disableForm from "../mixins/submitDisableMixin";

export default {
    mixins: [disableForm],
    components: {
        questionCard
    },
    props: {
        user: {},
        cardList: {
            type: Array,
        },
    },
    data() {
        return {
            showDialog: false,
            interval: 700000,
            typeAnswer: false
        }
    },
    beforeRouteLeave(to, from, next) {
        this.resetQuestion();
        next()
    },
    methods: {
        ...mapActions(["resetQuestion"]),
        requestDialogClose() {
            this.$root.$emit('closePopUp', 'suggestions')
        },
        answerMore(id) {
            this.$router.push({name: 'question', params: {id: `${id}`}});
            console.log(this.$router);
            this.requestDialogClose();
        }
    },
    computed: {},

}