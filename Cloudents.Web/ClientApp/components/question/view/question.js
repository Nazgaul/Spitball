import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
import questionCard from "./../helpers/question-card/question-card.vue";
import miniChat from "./../../chat/private-chat/private-chat.vue";

export default {
    components: {questionTextArea, questionCard, miniChat},
    data() {
        return {
            selectItems: ["Phisics", "Math", "Geography"],
            category: '',
            questionText: '',
            price: 0.5,
            tabs:null
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
        },
        isMobile(){return this.$vuetify.breakpoint.xsOnly;}
    }
}