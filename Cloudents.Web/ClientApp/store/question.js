import questionService from '../services/questionService'
const state={
 correctAnswer:"",
    deletedAnswer:false
}
const mutations={
    updateAnswer(state,data){
        state.correctAnswer=data;
    },
    updateDeleted(state,data){
        state.deletedAnswer=data;
    }
}
const getters={
    getCorrectAnswer:(state)=>state.correctAnswer,
    isDeletedAnswer:(state)=>state.deletedAnswer
}
const actions = {
    resetQuestion({commit}){
        commit('updateDeleted',false);
        commit('updateAnswer','');

    },
    deleteQuestion(context,id){
       if(id.type==='Answer') {context.commit('updateDeleted',true)}
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