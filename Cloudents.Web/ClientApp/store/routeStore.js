import analyticsService from '../services/analytics.service'

const state = {
    routeStack: [],
};

const getters = {
    getRouteStack: state => state.routeStack,
};

const mutations = {
    setRouteStack(state, val){
        state.routeStack.push(val);
    }
};

const actions = {
    setRouteStack({commit}, val){
        commit('setRouteStack', val);
    },
    sendQueryToAnalytic(context, to) {
        let queryString = '';
        let queries = to.query;
        for(let query in queries) {
            queryString += `${query}=${queries[query]}|`;
        }
        analyticsService.sb_unitedEvent('user_location', to.path, queryString);
    },
    fireOptimizeActivate(){
        if(!!global.dataLayer){
            global.dataLayer.push({ event: "optimize.activate" });
        }
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}