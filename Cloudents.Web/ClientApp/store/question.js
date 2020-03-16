import questionService from '../services/questionService'

const state = {
    deletedAnswer: false,
    question: null,
    cardOwner: false
};
const mutations = {
    updateDeleted(state, data) {
        state.deletedAnswer = data;
    },
    updateQuestion(state, data){
        state.question = data;
    },
    addAnswer(state, answer){
        state.question.answers.push(answer);
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
    markAsCorrect(state, answerId){
        state.question.hasCorrectAnswer = true;
        state.question.correctAnswerId = answerId;
    },
};
const getters = {
    getCorrectAnswer: (state) => {
        if(!!state.question){
            return state.question.hasOwnProperty('correctAnswerId');
        }else{
            return false;
        }
    },
    isDeletedAnswer: (state) => state.deletedAnswer,
    getQuestion: (state) => state.question,
    isCardOwner: (state, {accountUser}) =>{
        if(!accountUser) return false;
        if(!state.question) return false;
        return accountUser.id === state.question.userId;
    }
};
const actions = {
    resetQuestion({commit}) {
        commit('updateDeleted', false);
        commit('updateQuestion', null);
    },
    removeDeletedAnswer({commit}) {
        commit('updateDeleted', false);
    },
    deleteQuestion(context, id) {
        return questionService.deleteQuestion(id).then(()=>{
            if (id.type === 'Answer') {
                context.commit('updateDeleted', true);
            }
        });
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