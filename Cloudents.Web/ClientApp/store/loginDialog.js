
const state = {
    loginDialogState: false
};
const mutations = {
    setLoginDialogState(state, data) {
        state.loginDialogState = data;
    },
};
const getters = {
    loginDialogState: (state) => state.loginDialogState,

};
const actions = {
    updateLoginDialogState({commit}, data) {
        commit('setLoginDialogState', data);
    },
};
export default {
    actions, state, mutations, getters
};