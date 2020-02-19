const state = {
    showBuyDialog: false
};
const mutations = {
    setShowBuyDialog(state, val){
        state.showBuyDialog = val;
    }
};
const getters = {
    getShowBuyDialog(state){
        return state.showBuyDialog;
    }
};
const actions = {
    updateShowBuyDialog({commit}, val){
        commit('setShowBuyDialog', val);
    }
};

export default {
    actions,
    state,
    getters,
    mutations
}