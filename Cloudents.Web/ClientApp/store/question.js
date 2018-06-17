import questionService from '../services/questionService'
const state={
 correctAnswer:""
}
const mutations={
    updateAnswer(state,data){
        state.correctAnswer=data;
    }
}
const getters={
    getCorrectAnswer:(state)=>state.correctAnswer
}
const actions = {
    deleteQuestion(context,id){
        return questionService.deleteQuestion(id);
    },
    correctAnswer(context,id){
        context.commit('updateAnswer',id);
        return questionService.markAsCorrectAnswer(id)
    }
}
export default {
    actions,state,mutations,getters
}