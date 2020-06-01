import questionService from '../services/questionService'

const state = {
    question: null,
    cardOwner: false
};
const mutations = {
    updateQuestion(state, data){
        state.question = data;
    },
    removeAnswer(state, answerId){
        let answerIndex = -1;
        state.question.answers.forEach((answer, index) => {
            if(answer.id === answerId){
                answerIndex = index;
                return;
            }
        });
        if(answerIndex > -1){
            state.question.answers.splice(answerIndex, 1);
        }
    },
};
const getters = {
    getQuestion: (state) => state.question,
    isCardOwner: (state, {accountUser}) =>{
        if(!accountUser) return false;
        if(!state.question) return false;
        return accountUser.id === state.question.userId;
    }
};
const actions = {
    resetQuestion({commit}) {
        commit('updateQuestion', null);
    },
    deleteQuestion(context, id) {
        return questionService.deleteQuestion(id);
    },
    setQuestion({commit}, id){
        return questionService.getQuestion(id)
        .then((response) => {
            commit('updateQuestion', response);
        }).catch(ex => {
            console.log(ex);
        });
    },
    answerRemoved({commit}, notifyObj){
        let questionId = notifyObj.questionId;
        let answerId = notifyObj.answer.id;
        //update question in case user is in the question page
        if(!!state.question && state.question.id === questionId){
            commit('removeAnswer', answerId);
         }         
    },
};
export default {
    actions, state, mutations, getters
}