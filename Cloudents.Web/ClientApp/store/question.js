import questionService from '../services/questionService'
import searchService from '../services/searchService'
import reputationService from '../services/reputationService'

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
    updateAnswerVotes(state, {id, type}){
        state.question.answers.forEach((answer) => {
            if(answer.id === id){
                reputationService.updateVoteCounter(answer, type);
            }
        });
    },
    updateInnerQuestionVotes(state, {id, type}){
        if(!!state.question && state.question.id === id){
            reputationService.updateVoteCounter(state.question, type);
        }
    }
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
    correctAnswer(context, id) {
        return questionService.markAsCorrectAnswer(id);
    },
    setQuestion({commit}, id){
        return questionService.getQuestion(id)
        .then((response) => {
            commit('updateQuestion', response);
        }).catch(ex => {
            console.log(ex);
        })
    },
    updateQuestionItemCorrect({commit, state}, question){
        if(!!state.question && state.question.id == question.questionId){
            commit('markAsCorrect', question.answerId);
        }
    },
    answerAdded({commit, state}, notifyObj){
        let questionId = notifyObj.questionId;
        let answerObj = searchService.createAnswerItem(notifyObj.answer);
        //update question in case user is in the question page
        if(!!state.question && state.question.id === questionId){
           commit('addAnswer', answerObj);
        }
    },
    answerRemoved({commit}, notifyObj){
        let questionId = notifyObj.questionId;
        let answerId = notifyObj.answer.id;
        //update question in case user is in the question page
        if(!!state.question && state.question.id === questionId){
            commit('removeAnswer', answerId);
         }         
    },
    answerVote({commit, dispatch}, data){
        reputationService.voteAnswer(data.id, data.type).then(()=>{
            commit('updateAnswerVotes', data);
        }, (err) => {
            let errorObj = {
                toasterText:err.response.data.Id[0],
                showToaster: true,
            };
            dispatch('updateToasterParams', errorObj);
        });
    },
    innerQuestionVote({commit}, data){
        commit('updateInnerQuestionVotes', data);
    }
};
export default {
    actions, state, mutations, getters
}