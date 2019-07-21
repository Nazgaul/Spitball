const state = {
    routeStack: []
};

const getters = {
    getRouteStack: state => state.routeStack
};

const mutations = {
    setRouteStack(state, val){
        state.routeStack.push(val);
    }
};

const actions = {
    setRouteStack({commit}, val){
        commit('setRouteStack', val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}