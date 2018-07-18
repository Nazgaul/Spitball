import questionCard from "../question/helpers/question-card/question-card.vue";
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
            this.$root.$emit('closeSuggestionPopUp')
        },
        answerMore(id) {
            this.requestDialogClose();
            //
            // this.$nextTick(function () {
            //     this.$router.push({path: '/question/' + id});
            //     this.$forceUpdate()
            // })
        }

    },
    computed: {
     },
    watch: {
        '$props':{
            handler: function (val, oldVal) {
                console.log('watch', val)
            },
            deep: true
        }
    },
    created() {
    }
}