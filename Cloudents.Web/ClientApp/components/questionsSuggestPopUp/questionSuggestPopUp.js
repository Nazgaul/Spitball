import { mapGetters } from 'vuex'
import questionCard from "../question/helpers/question-card/question-card.vue";

export default {
    components: {
        questionCard
    },
    props: {},
    data() {
        return {
            cardList: [{
                id: 1444,
                subject: "Computers and Technology",
                price: 20.00000,
                text: "sadsad",
                files: 0,
                answers: 0,
                user: {
                    id: 78,
                    name: "eidan.1391"
                },
                dateTime: "2018-06-28T11:47:53.3321068Z"
            }, {
                id: 1443,
                subject: "Languages",
                price: 12.00000,
                text: "sadasdas",
                files: 0,
                answers: 0,
                user: {
                    id: 78,
                    name: "eidan.1391"
                },
                dateTime: "2018-06-28T11:44:14.8843316Z"
            }, {
                id: 1442,
                subject: "Arts",
                price: 20.00000,
                text: "sadfasd",
                files: 0,
                answers: 0,
                user: {
                    id: 78,
                    name: "eidan.1391"
                },
                dateTime: "2018-06-28T11:43:10.3948969Z"
            }, {
                id: 1441,
                subject: "Health",
                price: 20.00000,
                text: "What is what",
                files: 0,
                answers: 0,
                user: {
                    id: 78,
                    name: "eidan.1391"
                },
                dateTime: "2018-06-28T11:42:13.1962119Z"
            }, {
                id: 1432,
                subject: "Business",
                price: 40.00000,
                text: "How do you start a business?",
                files: 0,
                answers: 0,
                user: {
                    id: 550,
                    name: "Tori Withee.4380"
                },
                dateTime: "2018-06-27T11:11:56.4353445Z"
            }, {
                id: 1427,
                subject: "Mathematics",
                price: 20.00000,
                text: "For test",
                files: 0,
                answers: 2,
                user: {
                    id: 573,
                   name: "oliver2.3332"
                },
                dateTime: "2018-06-26T19:28:59.5849068Z"
            }, ]
        }

    },
    methods: {},
    computed: {

    },

    created() {


    }
}