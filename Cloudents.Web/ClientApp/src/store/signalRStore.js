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
        return
    },
    signalR_reconnect() {
        return
    },
    signalR_disconnect() {
        return
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