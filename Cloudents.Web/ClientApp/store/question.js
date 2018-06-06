import questionService from '../services/questionService'
const actions = {
    deleteQuestion(context,id){
        return questionService.deleteQuestion(id);
    }
}
export default {
    actions
}