import questionService from '../services/questionService'
import searchService from '../services/searchService'

const state = {
    deletedAnswer: false,
    question: null,
    cardOwner: false,
};
const mutations = {
    updateDeleted(state, data) {
        state.deletedAnswer = data;
    },
    updateQuestion(state, data){
        state.question = data;
    }
};
const getters = {
    getCorrectAnswer: (state) => state.question.hasOwnProperty('correctAnswerId'),
    isDeletedAnswer: (state) => state.deletedAnswer,
    getQuestion: (state) => state.question,
    isCardOwner: (state, {accountUser}) =>{
        if(!accountUser) return false;

        return accountUser.id === state.question.user.id;
    }
};
const actions = {
    resetQuestion({commit}) {
        commit('updateDeleted', false);
        commit('updateQuestion', null)
    },
    removeDeletedAnswer({commit}) {
        commit('updateDeleted', false);
    },
    deleteQuestion(context, id) {
        if (id.type === 'Answer') {
            context.commit('updateDeleted', true)
        }
        return questionService.deleteQuestion(id);
    },
    correctAnswer(context, id) {
        return questionService.markAsCorrectAnswer(id)
    },
    setQuestion({commit}, id){
        return questionService.getQuestion(id)
        .then((response) => {
            commit('updateQuestion', response);
        })
    },
    updateQuestionSignalR({commit, state}, question){
        if(!!state.question && state.question.id === question.id){
            let res = searchService.createQuestionItem(question);
            commit('updateQuestion', res);
        }
        
    }
};
export default {
    actions, state, mutations, getters
}