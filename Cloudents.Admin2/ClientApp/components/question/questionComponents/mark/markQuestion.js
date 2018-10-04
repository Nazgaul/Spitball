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
            acceptQuestion(question){
                acceptAnswer(question.toServer()).then(()=>{
                    alert(`SUCCESS: question id: ${question.questionId} accepted answer id: ${question.answerId}`)
                }, ()=>{
                    alert(`ERROR FAILED TO ACCEPT question id: ${question.questionId} answer id: ${question.answerId}`)
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