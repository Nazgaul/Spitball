
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
        commit('setNewQuestionState', data);
    },
};
export default {
    actions, state, mutations, getters
};