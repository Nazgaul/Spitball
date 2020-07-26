const state = {
    isConnected: true
};

const getters = {
    getIsSignalRConnected:state => state.isConnected
};

const mutations = {
    setIsSignalRConnected(state, val){
        state.isConnected = val;
    },
    signalR_emit() {

    },
    signalR_reconnect() {

    },
    signalR_disconnect() {

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