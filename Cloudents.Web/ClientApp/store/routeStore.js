const state = {
    routeStack: [],
    isFrymo: global.siteName === 'frymo'
};

const getters = {
    getRouteStack: state => state.routeStack,
    isFrymo: state => state.isFrymo
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
    sendQueryToAnalytic({dispatch}, to) {
        let queryString = '';
        let queries = to.query;
        for(let query in queries) {
            queryString += `${query}=${queries[query]}|`;
        }
        dispatch('updateAnalytics_unitedEvent',['user_location', to.path, queryString])
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