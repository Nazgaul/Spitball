import questionCard from "../question/helpers/question-card/question-card.vue";

export default {
    components: {
        questionCard
    },
    props: {
        user: {}
    },
    data() {
        return {
            showDialog: false,
            interval: 7000,
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
                    "id":1397,
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
                    "id":1222,
                    "text":"triangle has a perimeter of 50. If two of the side lengths are equal and the third side is five more than the equal sides, what is the length of  the third side?",
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
    methods: {

        requestDialogClose(){
            console.log('sdfsdf')
            this.$root.$emit('closeSuggestionPopUp')
        },
        answerMore(id){
            console.log(id)
            this.requestDialogClose();
            this.$router.push({path: '/question/'+id});
        }

    },
    computed: {

    },

    created() {

    }
}