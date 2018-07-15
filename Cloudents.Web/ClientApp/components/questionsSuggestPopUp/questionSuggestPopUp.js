import questionCard from "../question/helpers/question-card/question-card.vue";
import {mapActions} from 'vuex'
export default {
    components: {
        questionCard
    },
    props: {
        user: {},
        cardList:{ type: Array, required: false},
    },
    data() {
        return {
            showDialog: false,
            interval: 7000,

        }

    },
    beforeRouteLeave(to, from, next) {
        this.resetQuestion();
        next()
    },
    methods: {
        ...mapActions(["resetQuestion"]),
        requestDialogClose() {
            console.log('sdfsdf')
            this.$root.$emit('closeSuggestionPopUp')
        },
        answerMore(id) {
            this.requestDialogClose();

            this.$nextTick(function () {
                this.$router.push({path: '/question/' + id});
                this.$forceUpdate()
            })
        }

    },
    computed: {},
    created() {

    }
}