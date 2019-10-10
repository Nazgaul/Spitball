const state = {
    isConnected: true
};

const getters = {
    getIsSignalRConnected:state => state.isConnected
};

const mutations = {
    setIsSignalRConnected(state, val){
        state.isConnected = val;
    }
};

const actions = {
    setIsSignalRConnected({commit}, val){
        commit('setIsSignalRConnected', val);
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}