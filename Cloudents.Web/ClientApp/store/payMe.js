const state = {
    showPayMeToaster: false
}
const mutations = {
    setShowPayMeToaster(state, val){
        state.showPayMeToaster = val;
    }
}
const getters = {
    getShowPayMeToaster(state){
        return state.showPayMeToaster
    }
}
const actions = {
    updateShowPayMeToaster({commit}, val){
        commit('setShowPayMeToaster', val);
    }
}

export default {
    actions,
    state,
    getters,
    mutations
}