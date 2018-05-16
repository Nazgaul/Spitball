import questionTextArea from "../questionTextArea.vue";

export default {
    components: {questionTextArea},
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