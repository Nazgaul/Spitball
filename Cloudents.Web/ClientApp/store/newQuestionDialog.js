import analyticsService from '../services/analytics.service'

const state = {
    newQuestionDialog: false,
};
const mutations = {
    setNewQuestionState(state, data) {
        state.newQuestionDialog = data;
    },
};
const getters = {
    newQuestionDialogSate: (state) => state.newQuestionDialog,

};
const actions = {
    updateNewQuestionDialogState({commit}, data) {
        //ab testing
        analyticsService.sb_unitedEvent('AB_TESTING', 'ASK_QUESTION_CLICKED')
        commit('setNewQuestionState', data);
    },
};
export default {
    actions, state, mutations, getters
};