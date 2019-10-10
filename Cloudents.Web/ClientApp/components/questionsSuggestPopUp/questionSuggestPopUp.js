import questionCard from "../question/helpers/new-question-card/new-question-card.vue";
import {mapActions} from 'vuex'

export default {
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
            typeAnswer: false,
            isRtl: global.isRtl

        };
    },
    beforeRouteLeave(to, from, next) {
        this.resetQuestion();
        next();
    },
    methods: {
        ...mapActions(["resetQuestion"]),
        requestDialogClose() {
            this.$root.$emit('closePopUp', 'suggestions');
        },
        answerMore(id) {
            this.$router.push({path: `/question/${id}`}) ;
            //this.$router.push({name: 'question', params: {id: `${id}`}});
            this.requestDialogClose();
        }
    },
    computed: {},

}