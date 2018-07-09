import questionCard from "../question/helpers/question-card/question-card.vue";

export default {
    components: {
        questionCard
    },
    props: {},
    data() {
        return {
            showDialog: false,
            cardList: [
                {
                    "subject":"History",
                    "id":1265,
                    "text":"test1",
                    "price":10,
                    "user":{
                        "id":638,
                        "name":"yaari.9181"
                    },
                    "answers":[
                    ],
                    "create":"2018-06-28T10:50:58.7196568Z",
                    "files":[
                    ],
                    "filesNum":0,
                    "answersNum":0,
                    "cardOwner":false
                },
                {
                    "subject":"History",
                    "id":1265,
                    "text":"test2",
                    "price":10,
                    "user":{
                        "id":638,
                        "name":"yaari.9181"
                    },
                    "answers":[
                    ],
                    "create":"2018-06-28T10:50:58.7196568Z",
                    "files":[
                    ],
                    "filesNum":0,
                    "answersNum":0,
                    "cardOwner":false
                },
                {
                    "subject":"History",
                    "id":1265,
                    "text":"test3",
                    "price":10,
                    "user":{
                        "id":638,
                        "name":"yaari.9181"
                    },
                    "answers":[
                    ],
                    "create":"2018-06-28T10:50:58.7196568Z",
                    "files":[
                    ],
                    "filesNum":0,
                    "answersNum":0,
                    "cardOwner":false
                },
            ]
        }

    },
    methods: {},
    computed: {

    },

    created() {

    }
}