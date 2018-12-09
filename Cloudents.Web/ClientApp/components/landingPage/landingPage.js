import questionCard from '../question/helpers/question-card/question-card.vue';
import extendedTextArea from '../question/helpers/extended-text-area/extendedTextArea.vue'
import newQuestion from '../question/newQuestion/newQuestion.vue'
export default {
    name: "landingPage",
    components: {
        questionCard,
        extendedTextArea,
        newQuestion
    },
    data() {
        return {
            randomQuestionData: {
                color: 'green',
                user: {

                },
                subject: 'test subject',
                price: '90'
            },
            value: {
                type: String
            },
            error: {},
            actionType: 'answer',
            isFocused: false,
            uploadUrl: {
                type: String
            },
            textAreaValue: '',
            errorTextArea: {},
            isRtl: global.isRtl,
            //TODO mock data change to api call to get random questions
            cardList: [
                {
                    "id": 0,
                    "subject": "mathematics",
                    "price": 12,
                    "text": "test one",
                    "files": 0,
                    "answers": 0,
                    "user": {
                        "id": 0,
                        "name": "string",
                        "image": "string"
                    },
                    "dateTime": "2018-12-09T12:53:34.086Z",
                    "color": "default",
                    "hasCorrectAnswer": false,
                    "isRtl": true
                },
                {
                    "id": 1,
                    "subject": "Biology",
                    "price": 10,
                    "text": "test one test onetest onetest one",
                    "files": 0,
                    "answers": 0,
                    "user": {
                        "id": 0,
                        "name": "string",
                        "image": "string"
                    },
                    "dateTime": "2018-12-09T12:53:34.086Z",
                    "color": "default",
                    "hasCorrectAnswer": false,
                    "isRtl": true
                },
                {
                    "id": 2,
                    "subject": "History",
                    "price": 20,
                    "text": "test onetest onetest onetest onetest one",
                    "files": 0,
                    "answers": 0,
                    "user": {
                        "id": 0,
                        "name": "string",
                        "image": "string"
                    },
                    "dateTime": "2018-12-09T12:53:34.086Z",
                    "color": "default",
                    "hasCorrectAnswer": false,
                    "isRtl": true
                }
            ],
        }
    },
    props: {
        propName: {
            type: Number,
            default: 0
        },
    },
    computed: {
        name() {
            return this;
        }

    },
    methods: {
        name() {

        }
    },
    created() {

    },
}