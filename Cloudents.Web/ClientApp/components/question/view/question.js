import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionCard from "./../helpers/question-card/question-card.vue";
import miniChat from "./../../chat/view/chat.vue";

export default {
    components: {questionTextArea, questionCard},
    data() {
        return {
            selectItems: ["Phisics", "Math", "Geography"],
            category: '',
            questionText: '',
            price: 0.5
        }
    },
    methods:{
        ask(){
            console.log(this.price,this.questionText, this.category)
        }
    },
    computed:{
        validForm(){
            return (this.category && this.questionText.length && this.price >=0.5) ? true : false
        }
    }
}