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
            this.$router.push({path: `/question/${id}`},
                function(success){console.log("success: " + success)},
                function(abort){console.log("abort: " + abort)} );
            this.requestDialogClose();
        }
    },
    computed: {},

}