import questionCard from '../question/helpers/question-card/question-card.vue';
import extendedTextArea from '../question/helpers/extended-text-area/extendedTextArea.vue'
export default {
    name: "landingPage",
    components:{
        questionCard,
        extendedTextArea
    },
    data() {
        return {
            randomQuestionData:{
                color: 'green',
                user:{

                },
                subject:'test subject',
                price: '90'
            },
            value: {type: String},
            error: {},
            actionType:  'answer',
            isFocused: false,
            uploadUrl: {type: String},
            textAreaValue: '',
            errorTextArea:{}
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
}