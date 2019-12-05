const state = {
    touchState:{
        isTouchStarted: false,
        isTouchMove: false,
        isTouchEnded: false,
        
    }
};

const mutations = {
    setIsTouchStarted(state, val){
        state.touchState.isTouchStarted = val;
    },
    setIsTouchMove(state, val){
        state.touchState.isTouchMove = val;
    },
    setIsTouchEnd(state, val){
        state.touchState.isTouchEnd = val;
    },
};

const getters = {
    getIsTouchStarted: state=>state.touchState.isTouchStarted,
    getIsTouchMove: state=>state.touchState.isTouchMove,
    getIsTouchEnd: state=>state.touchState.isTouchEnd,
};

const actions = {
    setIsTouchStarted({commit}, val){
        commit('setIsTouchStarted', val)
    },
    setIsTouchMove({commit}, val){
        commit('setIsTouchMove', val)
    },
    setIsTouchEnd({commit}, val){
        commit('setIsTouchEnd', val)
    },
};

export default {
    state,
    getters,
    actions,
    mutations
}
