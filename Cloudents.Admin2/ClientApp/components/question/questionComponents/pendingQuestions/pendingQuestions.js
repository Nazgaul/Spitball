import {getAllQuesitons, aproveQuestion} from './pendingQuestionsService'
import {deleteQuestion} from '../delete/deleteQuestionService'

export default {
    data(){
        return {
            questions:[],
            deletedMails: []
        }
    },
    methods:{     
        aproveQ(question, index){
            aproveQuestion(question.id).then(()=>{
                this.questions.splice(index, 1);
                this.$toaster.success(`Question Aproved`);
            }, ()=>{
                this.$toaster.error(`Question Aproved Failed`);
            })
        },
        declineQuestion(question, index){
            let id = question.id
            deleteQuestion([id]).then(()=>{
                this.deletedMails.push(question.user.email)
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
        })
    }
}