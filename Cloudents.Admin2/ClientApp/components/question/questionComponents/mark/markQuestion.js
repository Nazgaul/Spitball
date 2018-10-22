import { getAllQuesitons, acceptAnswer } from './markQuestionService'

export default {
    data(){
        return {
            questions:[]
        }
    },
    methods:{
            openQuestion(url){
                window.open(url, "_blank");
            },
            acceptQuestion(question, answer){
                acceptAnswer(question.toServer(answer.id)).then(()=>{
                    //alert(`SUCCESS: question id: ${question.id} accepted answer id: ${answer.id}`)
                    //remove the question from the list
                    this.$toaster.success(`Question id: ${question.id} accepted answer id: ${answer.id}`)
                    let questionIndex = this.questions.indexOf(question)
                    this.questions.splice(questionIndex, 1);
                }, ()=>{
                    this.$toaster.error(`ERROR FAILED TO ACCEPT question id: ${question.id} answer id: ${answer.id}`)
                })
            }
        
    },
    created(){
        getAllQuesitons().then((questionsResponse) => {
            this.questions = questionsResponse;
            console.log(questionsResponse);
        })
    }
}