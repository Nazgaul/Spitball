import { getAllQuesitons } from './markQuestionService'

export default {
    data(){
        return {
            questions:[]
        }
    },
    created(){
        getAllQuesitons().then((questionsResponse) => {
            this.questions = questionsResponse;
            console.log(questionsResponse);
        })
    }
}