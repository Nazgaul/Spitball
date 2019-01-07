import {getAllQuesitons, unflagQuestion} from './flaggedQuestionsService'
import {deleteQuestion} from '../delete/deleteQuestionService'

export default {
    data(){
        return {
            questions: [],
            loading: true
        }
    },
    methods:{     
        unflagQ(question, index){
            unflagQuestion(question.id).then(()=>{
                this.questions.splice(index, 1);
                this.$toaster.success(`Question Aproved`);
            }, ()=>{
                this.$toaster.error(`Question Aproved Failed`);
            })
        },
        declineQuestion(question, index){
            let id = question.id
            deleteQuestion([id]).then(()=>{
                this.questions.splice(index, 1);
                this.$toaster.success(`Question Declined`);
            }, err=>{
                this.$toaster.error(`Question Declined Failed`);
            })
        }   
    },
    created(){
        getAllQuesitons().then((questionsResp)=>{
            this.questions = questionsResp;
            this.loading = false;
        })
    }
}