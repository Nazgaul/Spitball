const state = {
   leaderBoardState: false
};

const getters = {
    getLeaderBoardState:  state => state.leaderBoardState,
};

const mutations = {
    setLeaderBoardState(state, val) {
        state.leaderBoardState = val
    },
};


const actions = {
    updateLeaderBoardState({commit}, val){
        commit('setLeaderBoardState', val);
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}